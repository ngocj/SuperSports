using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SP.Application.Dto.BrandDto;
using SP.Application.Dto.CartDto;
using SP.Application.Dto.CategoryDto;
using SP.Application.Dto.OrderDetailDto;
using SP.Application.Dto.OrderDto;
using SP.Application.Dto.ProductDto;
using SP.Application.Dto.ProductVariantDto;
using SP.Application.Dto.ProvinceDto;
using SP.Application.Dto.UserDto;
using SP.Domain.Entity;
using SP.WebApi.VnPay;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace SP.WebApp.Controllers
{
    public class OrderController : Controller
    {
        private const string ApiUrl = "https://localhost:7131/api/order";
        private const string ApiUrl1 = "https://localhost:7131/api/";
        private HttpClient _httpClient;

        public OrderController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();       
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Manager, Employee")]
        public async Task<IActionResult> DetailOrder(Guid id)
        {
            var response = await _httpClient.GetFromJsonAsync<OrderViewDto>($"{ApiUrl}/{id}");
            return View(response);
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> BuyNowCheckout(int productVariantId , string productName, OrderCreateDto orderCreateDto, int quantity)
        {
            try
            {
                // 2. Kiểm tra token người dùng
                var token = HttpContext.Session.GetString("JwtToken");
                if (string.IsNullOrEmpty(token))
                    return RedirectToAction("Login", "Auth");

                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);

                var userIdClaim = jwt.Claims.FirstOrDefault(x => x.Type == "nameid");
                var userNameClaim = jwt.Claims.FirstOrDefault(x => x.Type == "unique_name");

                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId) || userNameClaim == null)
                    return RedirectToAction("Login", "Auth");

                string userNameFromToken = userNameClaim.Value;

                // 3. Lấy thông tin sản phẩm
                var productResponse = await _httpClient.GetAsync($"{ApiUrl1}productvariant/{productVariantId}");
                if (!productResponse.IsSuccessStatusCode)
                {
                    TempData["Error"] = "❌ Không tìm thấy sản phẩm";
                    return RedirectToAction("Index", "Home");
                }

                var productVariant = await productResponse.Content.ReadFromJsonAsync<VariantViewDto>();
                if (productVariant == null)
                {
                    TempData["Error"] = "❌ Thông tin sản phẩm không hợp lệ";
                    return RedirectToAction("Index", "Home");
                }

                // 4. Kiểm tra số lượng tồn kho
                if (quantity > productVariant.Quantity)
                {
                    TempData["Error"] = $"❌ Số lượng đặt hàng ({quantity}) vượt quá số lượng tồn kho ({productVariant.Quantity})";
                    return RedirectToAction("Index", "Home");
                }

                // 5. Lấy thông tin người dùng
                var userResponse = await _httpClient.GetAsync($"{ApiUrl1}user/{userId}");
                if (!userResponse.IsSuccessStatusCode)
                {
                    TempData["Error"] = "❌ Không lấy được thông tin người dùng";
                    return RedirectToAction("Login", "Auth");
                }

                var userInfo = await userResponse.Content.ReadFromJsonAsync<UserViewDto>();
                if (userInfo == null)
                {
                    TempData["Error"] = "❌ Thông tin người dùng không hợp lệ";
                    return RedirectToAction("Login", "Auth");
                }

                // 6. Chuẩn bị OrderDetails
                var orderDetails = new List<OrderDetailCreateDto>
                {
                    new OrderDetailCreateDto
                    {
                     ProductVariantId = productVariantId,
                     Quantity = quantity,
                    Price = productVariant.Price,
                    ProductVariant = productVariant,
                
                    }
                };
                // 1. Lấy Ward
                Ward? ward = null;
                int? districtId = null;
                int? provinceId = null;

                if (userInfo.WardId != null)
                {
                    // 1. Lấy Ward
                    var wardResponse = await _httpClient.GetAsync($"{ApiUrl1}Address/ward/{userInfo.WardId}");
                    if (wardResponse.IsSuccessStatusCode)
                    {
                        ward = await wardResponse.Content.ReadFromJsonAsync<Ward>();
                        districtId = ward?.DistrictId;

                        // 2. Lấy District nếu có
                        if (districtId != null)
                        {
                            var districtResponse = await _httpClient.GetAsync($"{ApiUrl1}Address/district/{districtId}");
                            if (districtResponse.IsSuccessStatusCode)
                            {
                                var district = await districtResponse.Content.ReadFromJsonAsync<District>();
                                provinceId = district?.ProvinceId;
                            }
                        }
                    }
                }

                // 3. Tạo mới OrderCreateDto
                var orderDto = new OrderCreateDto
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    UserName = userInfo.UserName,
                    PhoneNumber = userInfo.PhoneNumber,
                    AddressDetail = userInfo.AddressDetail,
                    Status = OrderStatus.Pending,
                    OrderDetails = orderDetails,
                    WardId = userInfo.WardId,
                    DistrictId = districtId,
                    ProvinceId = provinceId,
                    User = userInfo,
                    TotalPrice = productVariant.Price * quantity,
                    ProductName = productName
                };


                // 8. Gửi các ViewData cần thiết
                var categories = await _httpClient.GetFromJsonAsync<IEnumerable<CategoryViewDto>>($"{ApiUrl1}category");
                ViewBag.Categories = categories != null ? new SelectList(categories, "Id", "CategoryName") : null;

                var provinces = await _httpClient.GetFromJsonAsync<IEnumerable<Province>>($"{ApiUrl1}Address/provinces");
                ViewBag.Provinces = provinces != null ? new SelectList(provinces, "Id", "Name") : null;

                // 9. Trả về View thanh toán với thông tin đã có
                return View(orderDto);
            }
            catch (Exception)
            {
                TempData["Error"] = "❌ Đã xảy ra lỗi khi xử lý yêu cầu";
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> CartCheckout1(List<int> SelectedItems, List<CartViewDto> CartItems)
        {
            try
            {
                var token = HttpContext.Session.GetString("JwtToken");
                if (string.IsNullOrEmpty(token))
                {
                    TempData["Error"] = "❌ Vui lòng đăng nhập để tiếp tục";
                    return RedirectToAction("Login", "Auth");
                }

                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                var userIdClaim = jwt.Claims.FirstOrDefault(x => x.Type == "nameid");

                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    TempData["Error"] = "❌ Phiên đăng nhập không hợp lệ";
                    return RedirectToAction("Login", "Auth");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Lấy toàn bộ giỏ hàng
                var cartResponse = await _httpClient.GetAsync($"{ApiUrl1}cart/user/{userId}");
                if (!cartResponse.IsSuccessStatusCode)
                {
                    TempData["Error"] = "❌ Không lấy được giỏ hàng";
                    return RedirectToAction("GetAllCartByUserId", "Cart");
                }

                var allCartItems = await cartResponse.Content.ReadFromJsonAsync<List<CartViewDto>>();

                // Lọc ra các sản phẩm được chọn
                var selectedCartItems = allCartItems.Where(x => SelectedItems.Contains(x.ProductVariantId)).ToList();

                if (!selectedCartItems.Any())
                {
                    TempData["Error"] = "❌ Vui lòng chọn ít nhất một sản phẩm để thanh toán";
                    return RedirectToAction("GetAllCartByUserId", "Cart");
                }

                // Ánh xạ số lượng từ CartItems
                foreach (var item in selectedCartItems)
                {
                    var matching = CartItems.FirstOrDefault(ci => ci.ProductVariantId == item.ProductVariantId);
                    if (matching != null)
                    {
                        item.Quantity = matching.Quantity;
                    }
                }

                // Tạo danh sách OrderDetail và tính tổng tiền
                var orderDetails = new List<OrderDetailCreateDto>();
                decimal totalPrice = 0;

                foreach (var item in selectedCartItems)
                {
                    
                    if (item.Quantity > item.ProductVariant.Quantity)
                    {
                        TempData["Error"] = $"❌ Sản phẩm {item.ProductVariant.ProductName} vượt quá tồn kho";
                        return RedirectToAction("GetAllCartByUserId", "Cart");
                    }

                    orderDetails.Add(new OrderDetailCreateDto
                    {
                        ProductVariantId = item.ProductVariantId,
                        Quantity = item.Quantity,
                        Price = item.ProductVariant.Price,
                        ProductVariant = item.ProductVariant
                    });

                    totalPrice += item.ProductVariant.Price * item.Quantity;
                }

                // Lấy thông tin user và địa chỉ
                var userResponse = await _httpClient.GetAsync($"{ApiUrl1}user/{userId}");
                var userInfo = await userResponse.Content.ReadFromJsonAsync<UserViewDto>();

                int? districtId = null, provinceId = null;
                if (userInfo.WardId != null)
                {
                    var wardResponse = await _httpClient.GetAsync($"{ApiUrl1}Address/ward/{userInfo.WardId}");
                    var ward = await wardResponse.Content.ReadFromJsonAsync<Ward>();
                    districtId = ward?.DistrictId;

                    if (districtId != null)
                    {
                        var districtResponse = await _httpClient.GetAsync($"{ApiUrl1}Address/district/{districtId}");
                        var district = await districtResponse.Content.ReadFromJsonAsync<District>();
                        provinceId = district?.ProvinceId;
                    }
                }

                // Tạo đơn hàng
                var orderDto = new OrderCreateDto
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    UserName = userInfo.UserName,
                    PhoneNumber = userInfo.PhoneNumber,
                    AddressDetail = userInfo.AddressDetail,
                    Status = OrderStatus.Pending,
                    WardId = userInfo.WardId,
                    DistrictId = districtId,
                    ProvinceId = provinceId,
                    OrderDetails = orderDetails,
                    User = userInfo,
                    TotalPrice = totalPrice
                };

                // Gửi ViewData
                var categories = await _httpClient.GetFromJsonAsync<IEnumerable<CategoryViewDto>>($"{ApiUrl1}category");
                ViewBag.Categories = categories != null ? new SelectList(categories, "Id", "CategoryName") : null;

                var provinces = await _httpClient.GetFromJsonAsync<IEnumerable<Province>>($"{ApiUrl1}Address/provinces");
                ViewBag.Provinces = provinces != null ? new SelectList(provinces, "Id", "Name") : null;

                return View("CartCheckout", orderDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                TempData["Error"] = "❌ Đã xảy ra lỗi";
                return RedirectToAction("GetAllCartByUserId", "Cart");
            }
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CartCheckout(OrderCreateDto orderCreateDto, string paymentMethod, List<int> selectedProductVariantIds)
        {
            const int MaxQuantityPerItem = 5;

            try
            {
                // 1. Lấy token người dùng từ session
                var token = HttpContext.Session.GetString("JwtToken");
                if (string.IsNullOrEmpty(token))
                {
                    TempData["Error"] = "❌ Vui lòng đăng nhập để tiếp tục";
                    return RedirectToAction("Login", "Auth");
                }

                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                var userIdClaim = jwt.Claims.FirstOrDefault(x => x.Type == "nameid");
                var userNameClaim = jwt.Claims.FirstOrDefault(x => x.Type == "unique_name");

                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId) || userNameClaim == null)
                {
                    TempData["Error"] = "❌ Phiên đăng nhập không hợp lệ";
                    return RedirectToAction("Login", "Auth");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // 2. Lấy giỏ hàng của user
                var cartResponse = await _httpClient.GetAsync($"{ApiUrl1}cart/user/{userId}");
                if (!cartResponse.IsSuccessStatusCode)
                {
                    TempData["Error"] = "❌ Không lấy được giỏ hàng";
                    return RedirectToAction("CartCheckout", "Order");
                }

                var cartItems = await cartResponse.Content.ReadFromJsonAsync<List<CartViewDto>>();
                if (cartItems == null || !cartItems.Any())
                {
                    TempData["Error"] = "❌ Giỏ hàng của bạn đang trống";
                    return RedirectToAction("CartCheckout", "Order");
                }

                // Lọc ra chỉ những sản phẩm được chọn
                var selectedCartItems = cartItems.Where(x => selectedProductVariantIds.Contains(x.ProductVariantId)).ToList();

                if (!selectedCartItems.Any())
                {
                    TempData["Error"] = "❌ Không có sản phẩm nào được chọn để thanh toán";
                    return RedirectToAction("CartCheckout", "Order");
                }

                // 3. Tạo ID đơn hàng mới, gán thông tin người dùng và trạng thái
                orderCreateDto.Id = Guid.NewGuid();
                orderCreateDto.UserId = userId;
                orderCreateDto.UserName = userNameClaim.Value;
                orderCreateDto.Status = OrderStatus.Pending;
                orderCreateDto.PaymentMethod = paymentMethod == "VNPay" ? PaymentMethod.VnPay : PaymentMethod.Cash;

                // 4. Chuẩn bị danh sách OrderDetails và tính tổng tiền
                var orderDetails = new List<OrderDetailCreateDto>();
                decimal totalPrice = 0;

                foreach (var item in selectedCartItems)
                {
                    if (item.Quantity <= 0 || item.Quantity > MaxQuantityPerItem)
                    {
                        TempData["Error"] = $"❌ Số lượng của sản phẩm (ID: {item.ProductVariantId}) không hợp lệ";
                        return RedirectToAction("CartCheckout", "Order");
                    }

                    if (item.Quantity > item.ProductVariant.Quantity)
                    {
                        TempData["Error"] = $"❌ Số lượng đặt hàng ({item.Quantity}) của sản phẩm {item.ProductVariant.ProductName} vượt quá tồn kho ({item.ProductVariant.Quantity})";
                        return RedirectToAction("CartCheckout", "Order");
                    }

                    orderDetails.Add(new OrderDetailCreateDto
                    {
                        OrderId = orderCreateDto.Id,
                        ProductVariantId = item.ProductVariantId,
                        Quantity = item.Quantity,
                        Price = item.ProductVariant.Price
                    });

                    totalPrice += item.ProductVariant.Price * item.Quantity;
                }

                orderCreateDto.TotalPrice = totalPrice;
                orderCreateDto.OrderDetails = orderDetails;

                // 5. Gửi request tạo đơn hàng
                var orderResponse = await _httpClient.PostAsJsonAsync(ApiUrl, orderCreateDto);
                if (!orderResponse.IsSuccessStatusCode)
                {
                    TempData["Error"] = "❌ Tạo đơn hàng thất bại. Vui lòng thử lại";
                    return RedirectToAction("CartCheckout", "Order");
                }

                // 6. Chỉ xóa những sản phẩm đã được chọn khỏi giỏ hàng
                foreach (var item in selectedCartItems)
                {
                    var deleteResponse = await _httpClient.DeleteAsync($"{ApiUrl1}cart/{userId}/{item.ProductVariantId}");
                    if (!deleteResponse.IsSuccessStatusCode)
                    {
                        // Có thể log lỗi, không chặn quá trình
                        Console.WriteLine($"Không thể xóa sản phẩm {item.ProductVariantId} khỏi giỏ hàng.");
                    }
                }

                // Xử lý theo phương thức thanh toán
                if (paymentMethod == "VNPay")
                {
                    var paymentModel = new PaymentInformationModel
                    {
                        Amount = orderCreateDto.TotalPrice,
                        Name = orderCreateDto.UserName,
                        OrderType = "Mua hàng online",
                        OrderDescription = $"Thanh toán đơn hàng {orderCreateDto.ProductName}",
                        OrderId = orderCreateDto.Id
                    };

                    var paymentResponse = await _httpClient.PostAsJsonAsync($"{ApiUrl1}payment/create-payment-url", paymentModel);
                    if (!paymentResponse.IsSuccessStatusCode)
                    {
                        TempData["Error"] = "❌ Không thể khởi tạo thanh toán";
                        return RedirectToAction("Index", "Home");
                    }

                    var paymentUrl = await paymentResponse.Content.ReadAsStringAsync();
                    return Redirect(paymentUrl);
                }
                else // Thanh toán tiền mặt
                {
                    TempData["Success"] = "✅ Đặt hàng thành công! Chúng tôi sẽ liên hệ với bạn để xác nhận đơn hàng";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CartCheckout Error] {ex}");
                TempData["Error"] = "❌ Đã xảy ra lỗi khi đặt hàng. Vui lòng liên hệ hỗ trợ";
                return RedirectToAction("CartCheckout", "Order");
            }
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Payment(int productVariantId, OrderCreateDto orderCreateDto, int quantity, string paymentMethod)
        {
            try
            {
                if (quantity <= 0)
                {
                    TempData["Error"] = "❌ Số lượng đặt hàng phải lớn hơn 0";
                    return RedirectToAction("Index");
                }

                var token = HttpContext.Session.GetString("JwtToken");
                if (string.IsNullOrEmpty(token))
                    return RedirectToAction("Login", "Auth");

                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);

                var userIdClaim = jwt.Claims.FirstOrDefault(x => x.Type == "nameid");
                var userNameClaim = jwt.Claims.FirstOrDefault(x => x.Type == "unique_name");

                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId) || userNameClaim == null)
                    return RedirectToAction("Login", "Auth");

                string userNameFromToken = userNameClaim.Value;

                // Gán thông tin đơn hàng
                orderCreateDto.Id = Guid.NewGuid();
                orderCreateDto.UserId = userId;
                orderCreateDto.UserName = userNameFromToken;
                orderCreateDto.Status = OrderStatus.Pending;
                orderCreateDto.PaymentMethod = paymentMethod == "VNPay" ? PaymentMethod.VnPay : PaymentMethod.Cash;

                var productResponse = await _httpClient.GetAsync($"{ApiUrl1}productvariant/{productVariantId}");
                if (!productResponse.IsSuccessStatusCode)
                {
                    TempData["Error"] = "❌ Không tìm thấy sản phẩm";
                    return RedirectToAction("Index", "Home");
                }

                var productVariant = await productResponse.Content.ReadFromJsonAsync<VariantViewDto>();
                if (productVariant == null)
                {
                    TempData["Error"] = "❌ Thông tin sản phẩm không hợp lệ";
                    return RedirectToAction("Index", "Home");
                }

                if (productVariant.Quantity < quantity)
                {
                    TempData["Error"] = $"❌ Chỉ còn {productVariant.Quantity} sản phẩm trong kho";
                    return RedirectToAction("Index", "Home");
                }

                orderCreateDto.TotalPrice = productVariant.Price * quantity;
                orderCreateDto.OrderDetails = new List<OrderDetailCreateDto>
        {
            new OrderDetailCreateDto
            {
                OrderId = orderCreateDto.Id,
                ProductVariantId = productVariantId,
                Price = productVariant.Price,
                Quantity = quantity
            }
        };

                // Gửi request tạo đơn hàng
                var orderResponse = await _httpClient.PostAsJsonAsync(ApiUrl, orderCreateDto);
                if (!orderResponse.IsSuccessStatusCode)
                {
                    TempData["Error"] = "❌ Tạo đơn hàng thất bại. Vui lòng thử lại";
                    return RedirectToAction("Index", "Home");
                }

                // Xử lý theo phương thức thanh toán
                if (paymentMethod == "VNPay")
                {
                    // ✅ Gửi yêu cầu tạo URL thanh toán từ API
                    var paymentModel = new PaymentInformationModel
                    {
                        Amount = orderCreateDto.TotalPrice,
                        Name = orderCreateDto.UserName,
                        OrderType = "Mua hàng online",
                        OrderDescription = $"Thanh toán đơn hàng {orderCreateDto.ProductName}",
                        OrderId = orderCreateDto.Id
                    };

                    var paymentResponse = await _httpClient.PostAsJsonAsync($"{ApiUrl1}payment/create-payment-url", paymentModel);
                    if (!paymentResponse.IsSuccessStatusCode)
                    {
                        TempData["Error"] = "❌ Không thể khởi tạo thanh toán";
                        return RedirectToAction("Index", "Home");
                    }

                    var paymentUrl = await paymentResponse.Content.ReadAsStringAsync();
                    return Redirect(paymentUrl);

                }
                else // Thanh toán tiền mặt
                {
                    TempData["Success"] = "✅ Đặt hàng thành công! Chúng tôi sẽ liên hệ với bạn để xác nhận đơn hàng";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "❌ Đã xảy ra lỗi khi đặt hàng. Vui lòng liên hệ hỗ trợ";
                return RedirectToAction("Index", "Home");
            }
        }

        public  IActionResult Reponse()
        {
            TempData["Success"] = "✅ Đặt hàng thành công! Chúng tôi sẽ liên hệ với bạn để xác nhận đơn hàng";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetDistrictsByProvince(int provinceId)
        {
            var districts = await _httpClient.GetFromJsonAsync<IEnumerable<DistrictViewDto>>($"{ApiUrl1}Address/districts/{provinceId}");
            Console.WriteLine($"Districts count: {districts?.Count()}"); 
            return Json(districts);
        }

        [HttpGet]
        public async Task<IActionResult> GetWardsByDistrict(int districtId)
        {
            var wards = await _httpClient.GetFromJsonAsync<IEnumerable<WardViewDto>>($"{ApiUrl1}Address/wards/{districtId}");                      
            return Json(wards);
        }

        [Authorize(Roles = "Manager, Employee")]
        public async Task<IActionResult> UpdateOrder(Guid id)
        {
            var response = await _httpClient.GetFromJsonAsync<OrderUpdateDto>($"{ApiUrl}/{id}");
            return View(response);
        }

        [HttpPost]
        [Authorize(Roles = "Manager, Employee")]
        public async Task<IActionResult> UpdateOrder(OrderUpdateDto orderUpdate)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var userIdClaim = jwt.Claims.FirstOrDefault(c => c.Type == "nameid");
            var userNameClaim = jwt.Claims.FirstOrDefault(c => c.Type == "unique_name");
            var roleClaim = jwt.Claims.FirstOrDefault(c => c.Type == "role");

            if (userNameClaim == null)
                return RedirectToAction("Login", "Auth");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            if (!ModelState.IsValid)
                return View(orderUpdate);

            // Lấy trạng thái hiện tại của đơn hàng từ API (hoặc DB)
            var getOrderResponse = await _httpClient.GetAsync($"{ApiUrl}/{orderUpdate.Id}");
            if (!getOrderResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Đơn hàng không tồn tại hoặc lỗi lấy dữ liệu.");
                return View(orderUpdate);
            }

            var existingOrder = await getOrderResponse.Content.ReadFromJsonAsync<OrderViewDto>();
            if (existingOrder == null)
            {
                ModelState.AddModelError("", "Không lấy được thông tin đơn hàng.");
                return View(orderUpdate);
            }

            // Cấm chuyển trạng thái lùi
            if ((int)orderUpdate.Status < (int)existingOrder.Status)
            {
                ModelState.AddModelError("", "Không được phép chuyển trạng thái đơn hàng lùi lại.");
                return View(orderUpdate);
            }
            if (

        (existingOrder.Status == OrderStatus.Delivered ||
         existingOrder.Status == OrderStatus.Paid ||
         existingOrder.Status == OrderStatus.Shipping))
            {
                ModelState.AddModelError("", "Không thể hủy đơn hàng khi đơn hàng đang ở trạng thái Đã giao, Đã thanh toán hoặc Đang giao hàng.");
                return View(orderUpdate);
            }



            // Cấm chuyển trạng thái lùi hoặc nhảy trạng thái không liền kề
            if ((int)orderUpdate.Status != (int)existingOrder.Status + 1)
            {
                ModelState.AddModelError("", "Không được phép chuyển trạng thái đơn hàng nhảy trạng thái không liền kề.");
                return View(orderUpdate);
            }

            // Cập nhật trang thai don hang lan luot 
            if (orderUpdate.Status == OrderStatus.Confirmed && existingOrder.Status != OrderStatus.Pending)
            {
                ModelState.AddModelError("", "Chỉ có thể xác nhận đơn hàng đang ở trạng thái Chờ xác nhận.");
                return View(orderUpdate);
            }


            if (roleClaim != null && roleClaim.Value == "Employee" &&
                userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid employeeId))
            {
                orderUpdate.EmployeeId = employeeId;
            }

            var response = await _httpClient.PutAsJsonAsync(ApiUrl, orderUpdate);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Cập nhật đơn hàng thành công!";

                if (roleClaim != null && roleClaim.Value == "Manager")
                {
                    return RedirectToAction("GetAllOrder", "Manager");
                }
                else
                {
                    return RedirectToAction("GetAllOrder", "Employee");
                }
            }


            TempData["ErrorMessage"] = "Cập nhật không thành công.";
            return View(orderUpdate);
        }


        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"{ApiUrl}/{id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Xóa đơn hàng thành công.";
            }
            else
            {
                TempData["Error"] = "Xóa đơn hàng không thành công.";
            }
            return RedirectToAction("GetAllOrder", "Manager");

        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CancelOrder(Guid id)
        {
            try
            {             
                var response = await _httpClient.PutAsync($"{ApiUrl}/{id}/cancel", null);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "✅ Đơn hàng đã được hủy thành công";
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = $"❌ Không thể hủy đơn hàng";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error canceling order: {ex.Message}");
                TempData["Error"] = "❌ Đã xảy ra lỗi khi hủy đơn hàng";
            }

            return RedirectToAction("OrderHistory");
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> OrderHistory()
        {
            var categories = await _httpClient.GetFromJsonAsync<IEnumerable<CategoryViewDto>>($"{ApiUrl1}category");
            ViewBag.Categories = categories != null ? new SelectList(categories, "Id", "CategoryName") : null;
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var userIdClaim = jwt.Claims.FirstOrDefault(x => x.Type == "nameid");

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                return RedirectToAction("Login", "Auth");

            var orders = await _httpClient.GetFromJsonAsync<List<OrderViewDto>>($"{ApiUrl}/user/{userId}");

            return View(orders);
        }
    }
}

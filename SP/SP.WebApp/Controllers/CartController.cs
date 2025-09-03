using Microsoft.AspNetCore.Mvc;
using SP.Application.Dto.CartDto;
using SP.WebApp.Extensions;
using Microsoft.AspNetCore.Authorization;
using SP.Application.Dto.CategoryDto;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Azure;

namespace SP.WebApp.Controllers
{
    [Authorize(Roles = "User")]
    public class CartController : Controller
    {
        private const string ApiUrl = "https://localhost:7131/api/cart";
        private const string ApiUrl1 = "https://localhost:7131/api/";
        private readonly HttpClient _httpClient;

        public CartController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCartByUserId()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth"); 
            }

            Guid userId;
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);

                var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "nameid");
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out userId))
                {
                    return RedirectToAction("Login", "Auth");
                }
            }
            catch
            {
                return RedirectToAction("Login", "Auth"); // Token không hợp lệ
            }

            // Gán token vào Authorization Header
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Gọi API lấy danh mục
            var categories = await _httpClient.GetFromJsonAsync<IEnumerable<CategoryViewDto>>($"{ApiUrl1}category");
            if (categories == null || !categories.Any())
            {
                ModelState.AddModelError(string.Empty, "Không tìm thấy danh mục nào");
                return View();
            }

            ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");

            // Gọi API lấy giỏ hàng theo userId
            var carts = await _httpClient.GetFromJsonAsync<IEnumerable<CartViewDto>>($"{ApiUrl}/user/{userId}");
            return View(carts);
        }

        public async Task<IActionResult> Details(int id)
        {
            var cart = await _httpClient.GetFromJsonAsync<CartViewDto>($"{ApiUrl}/{id}");
            if (cart == null)
            {
                return NotFound();
            }
            return View(cart);
        }
   
        [HttpPost]
        public async Task<IActionResult> Create(CartCreateDto cartCreateDto)
        {
            // get user id from token
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "nameid");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return RedirectToAction("Login", "Auth");
            }
            cartCreateDto.UserId = userId;
            var response = await _httpClient.PostAsJsonAsync(ApiUrl, cartCreateDto);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "🛒 Thêm vào giỏ hàng thành công.";
            }
            else
            {
                TempData["Error"] = $"❌ Thêm vào giỏ hàng không thành công";
            }
            return RedirectToAction("GetAllCartByUserId", "Cart");  
         
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] CartUpdateDto cartUpdateDto)
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
                return Unauthorized();

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "nameid");

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                return Unauthorized();

            cartUpdateDto.UserId = userId;

            var response = await _httpClient.PutAsJsonAsync(ApiUrl, cartUpdateDto);

            if (response.IsSuccessStatusCode)
            {
                return Ok(); // Trả về 200
            }

            return BadRequest(); // Trả lỗi
        }

        public async Task<IActionResult> Delete(int id)
        {
            Guid UserId;
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }
            else {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "nameid");
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out UserId))
                {
                    return RedirectToAction("Login", "Auth");
                }
            }
            var response = await _httpClient.DeleteAsync($"{ApiUrl}/{UserId}/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "🗑️ Xóa thành công.";
            }
            else
            {
                TempData["Error"] = "❌ Xóa không thành công.";
            }

            return RedirectToAction("GetAllCartByUserId", "Cart");
        }

    }
}

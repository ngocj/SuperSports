using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Common;
using SP.Application.Dto.BrandDto;
using SP.Application.Dto.CategoryDto;
using SP.Application.Dto.DiscountDto;
using SP.Application.Dto.FeedbackDto;
using SP.Application.Dto.ImageDto;
using SP.Application.Dto.OrderDto;
using SP.Application.Dto.ProductDto;
using SP.Application.Dto.ProductVariantDto;
using SP.Application.Dto.UserDto;
using SP.Domain.Entity;
using System.Net.Http.Headers;
using static SP.Infrastructure.Repositories.Implement.ProductRepository;

namespace SP.WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private const string ApiUrl = "https://localhost:7131/api";
        private const string ApiUrl1 = "https://localhost:7131/api/product";
        private readonly HttpClient _httpClient;

        public AdminController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        public async Task<ActionResult> GetAllCategory()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<CategoryViewDto>>($"{ApiUrl}/category");
            return View(response);
        }
        public async Task<ActionResult> GetAllUser()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<UserViewDto>>($"{ApiUrl}/user");
            return View(response);
        }
        public async Task<ActionResult> GetAllProduct(int? brandId, int? SubcategoryId, bool? isActive)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var brands = await _httpClient.GetFromJsonAsync<IEnumerable<BrandViewDto>>($"{ApiUrl}/brand");
            if (brands == null || !brands.Any())
            {
                ModelState.AddModelError(string.Empty, "Không tìm thấy thương hiệu nào.");
                return View();
            }

            var subCategories = await _httpClient.GetFromJsonAsync<IEnumerable<SubCategoryViewDto>>($"{ApiUrl}/subcategory");
            if (subCategories == null || !subCategories.Any())
            {
                ModelState.AddModelError(string.Empty, "Không tìm thấy danh mục nào");
                return View();
            }

            ViewBag.Brands = new SelectList(brands, "Id", "BrandName", brandId);
            ViewBag.Categories = new SelectList(subCategories, "Id", "Name", SubcategoryId);
            ViewBag.IsActive = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Tất cả", Value = null },
                new SelectListItem { Text = "Đang hoạt động", Value = "true" },
                new SelectListItem { Text = "Ngừng hoạt động", Value = "false" }
            }, "Value", "Text", isActive);

            var response = await _httpClient.GetFromJsonAsync<IEnumerable<ProductViewDto>>
            ($"{ApiUrl}/product/filter?SubcategoryId={SubcategoryId}&brandId={brandId}&isActive={isActive}");

            ViewBag.SelectedBrandId = brandId;
            ViewBag.SelectedSubCategoryId = SubcategoryId;
            ViewBag.SelectedIsActive = isActive;
            return View(response);

        }
        public async Task<ActionResult> GetAllBrand()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<BrandViewDto>>($"{ApiUrl}/brand");
            return View(response);
        }
        public async Task<ActionResult> GetAllImage()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<ImageFileDto>>($"{ApiUrl}/imageFile");
            return View(response);
        }
        public async Task<ActionResult> GetAllProductVariant()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<VariantViewDto>>($"{ApiUrl}/productVariant");
            return View(response);
        }
        public async Task<ActionResult> GetAllFeedback()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<FeedbackViewDto>>($"{ApiUrl}/feedback");
            return View(response);
        }
        public async Task<IActionResult> GetProductCountByCategory()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<dynamic>>($"{ApiUrl1}/count-by-category");
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetProductCountByBrand()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<dynamic>>($"{ApiUrl1}/count-by-brand");
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetLowStockProducts(int threshold = 10)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<ProductViewDto>>($"{ApiUrl1}/low-stock?threshold={threshold}");
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetTopRevenueProducts(DateTime? fromDate, DateTime? toDate, int topCount = 10)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var queryString = new List<string>();
            if (fromDate.HasValue) queryString.Add($"fromDate={fromDate.Value:yyyy-MM-dd}");
            if (toDate.HasValue) queryString.Add($"toDate={toDate.Value:yyyy-MM-dd}");
            queryString.Add($"topCount={topCount}");

            var url = $"{ApiUrl}/top-revenue{(queryString.Any() ? "?" : "")}{string.Join("&", queryString)}";
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<ProductViewDto>>(url);
            return Json(result);
        }

        public class ProductStatsViewModel
        {
            public IEnumerable<LowStockProductDto> LowStockProducts { get; set; } = new List<LowStockProductDto>();
            public IEnumerable<ProductCountByCategoryDto> ProductsByCategory { get; set; } = new List<ProductCountByCategoryDto>();
            public IEnumerable<ProductCountByBrandDto> ProductsByBrand { get; set; } = new List<ProductCountByBrandDto>();
            public IEnumerable<TopRevenueProductDto> TopRevenueProducts { get; set; } = new List<TopRevenueProductDto>();
        }

        public async Task<ActionResult> Index()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var viewModel = new ProductStatsViewModel();

            // Lấy sản phẩm tồn kho thấp
            var lowStockProducts = await _httpClient.GetFromJsonAsync<IEnumerable<LowStockProductDto>>($"{ApiUrl1}/low-stock?threshold=10");
            viewModel.LowStockProducts = lowStockProducts ?? new List<LowStockProductDto>();

            // Lấy sản phẩm theo danh mục
            var productsByCategory = await _httpClient
                .GetFromJsonAsync<IEnumerable<ProductCountByCategoryDto>>($"{ApiUrl1}/count-by-category");
            viewModel.ProductsByCategory = productsByCategory ?? new List<ProductCountByCategoryDto>();

            // Lấy sản phẩm theo thương hiệu
            var productsByBrand = await _httpClient.GetFromJsonAsync<IEnumerable<ProductCountByBrandDto>>($"{ApiUrl1}/count-by-brand");
            viewModel.ProductsByBrand = productsByBrand ?? new List<ProductCountByBrandDto>();

            // Lấy sản phẩm doanh thu cao
            var topRevenueProducts = await _httpClient.GetFromJsonAsync<IEnumerable<TopRevenueProductDto>>($"{ApiUrl1}/top-revenue?topCount=5");
            viewModel.TopRevenueProducts = topRevenueProducts ?? new List<TopRevenueProductDto>();

            return View(viewModel);
        }
    }
}
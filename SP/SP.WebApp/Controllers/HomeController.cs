using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SP.Application.Dto.BrandDto;
using SP.Application.Dto.CategoryDto;
using SP.Application.Dto.DiscountDto;
using SP.Application.Dto.ProductDto;
using SP.WebApp.Models;
using System.Diagnostics;

namespace SP.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private const string ApiUrl = "https://localhost:7131/api/product";
        private const string ApiUrl1 = "https://localhost:7131/api/";
        private HttpClient _httpClient;
        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        public async Task<ActionResult> Index()
        {
            var categories = await _httpClient.GetFromJsonAsync<IEnumerable<CategoryViewDto>>($"{ApiUrl1}category");
            if (categories == null || !categories.Any())
            {
                ModelState.AddModelError(string.Empty, "Không tìm thấy danh mục nào");
                return View();
            }
            ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");
            // Lấy danh sách sản phẩm mới
            var newProducts = await _httpClient.GetFromJsonAsync<IEnumerable<ProductViewDto>>($"{ApiUrl}/top-newest");
            if (newProducts == null || !newProducts.Any())
                ModelState.AddModelError(string.Empty, "Không tìm thấy sản phẩm mới nào.");

            // Lấy danh sách sản phẩm bán chạy
            var bestSellingProducts = await _httpClient.GetFromJsonAsync<IEnumerable<ProductViewDto>>($"{ApiUrl}/top-best-selling");
            if (bestSellingProducts == null || !bestSellingProducts.Any())
                ModelState.AddModelError(string.Empty, "Không tìm thấy sản phẩm bán chạy nào.");

            // Truyền dữ liệu sang ViewBag
            ViewBag.NewProducts = newProducts ?? new List<ProductViewDto>();
            ViewBag.BestSellingProducts = bestSellingProducts ?? new List<ProductViewDto>();

            // Có thể load thêm tất cả sản phẩm nếu cần
            var products = await _httpClient.GetFromJsonAsync<IEnumerable<ProductViewDto>>(ApiUrl);

            return View(products ?? new List<ProductViewDto>());
        }

        [HttpGet]
        public async Task<ActionResult> Collections(string? search,decimal? priceFrom, decimal? priceTo, int? brandId, int? 
        subCategoryId, int? categoryId, string sort = "lastest")
        {
            // Gọi các API song song
            var brandTask = _httpClient.GetFromJsonAsync<IEnumerable<BrandViewDto>>($"{ApiUrl1}brand");
            var categoryTask = _httpClient.GetFromJsonAsync<IEnumerable<CategoryViewDto>>($"{ApiUrl1}category");
            var subCategoryTask = _httpClient.GetFromJsonAsync<IEnumerable<SubCategoryViewDto>>($"{ApiUrl1}subcategory");
            var discountTask = _httpClient.GetFromJsonAsync<IEnumerable<DiscountViewDto>>($"{ApiUrl1}Discount");

            await Task.WhenAll(brandTask, categoryTask, subCategoryTask, discountTask);

            var brands = await brandTask;
            var categories = await categoryTask;
            var subCategories = await subCategoryTask;
            var discounts = await discountTask;

            // ViewBag: Dropdowns
            ViewBag.Brands = new SelectList(brands, "Id", "BrandName");
            ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");
            ViewBag.Discounts = discounts.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = $"{d.Percent}%"
            }).ToList();

            // SubCategories theo category nếu có
            ViewBag.SubCategories = categoryId.HasValue
                ? subCategories.Where(sc => sc.CategoryId == categoryId.Value).ToList()
                : subCategories.ToList();

            // Gán lại filter params cho view
            ViewBag.BrandId = brandId;
            ViewBag.SubCategoryId = subCategoryId;
            ViewBag.CategoryId = categoryId;
            ViewBag.Sort = sort;
            // Tạo URL API với query string
            var query = new Dictionary<string, string?>
            {
                ["priceFrom"] = priceFrom?.ToString(),
                ["priceTo"] = priceTo?.ToString(),
                ["categoryId"] = categoryId?.ToString(),
                ["subCategoryId"] = subCategoryId?.ToString(),
                ["brandId"] = brandId?.ToString(),
                ["search"] = search,
            };

            var queryString = string.Join("&", query.Where(kv => !string.IsNullOrEmpty(kv.Value))
                                                    .Select(kv => $"{kv.Key}={kv.Value}"));
            string apiUrl = $"{ApiUrl}/{sort}?{queryString}";

            var products = await _httpClient.GetFromJsonAsync<IEnumerable<ProductViewDto>>(apiUrl);

            return View(products);
        }

        [HttpGet]
        public async Task<ActionResult> Detail(int id)
        {
            var product = await _httpClient.GetFromJsonAsync<ProductViewDto>($"{ApiUrl}/{id}");
            if (product == null)
            {
                ModelState.AddModelError(string.Empty, "Không tìm thấy sản phẩm.");
                return View();
            }

            // get all brands
            var brands = await _httpClient.GetFromJsonAsync<IEnumerable<BrandViewDto>>($"{ApiUrl1}brand");
            if (brands == null || !brands.Any())
            {
                ModelState.AddModelError(string.Empty, "Không tìm thấy thương hiệu nào.");
                return View();
            }

            // get all categories
            var categories = await _httpClient.GetFromJsonAsync<IEnumerable<CategoryViewDto>>($"{ApiUrl1}category");
            if (categories == null || !categories.Any())
            {
                ModelState.AddModelError(string.Empty, "Không tìm thấy danh mục nào");
                return View();
            }

            // get all discounts
            var discounts = await _httpClient.GetFromJsonAsync<IEnumerable<DiscountViewDto>>($"{ApiUrl1}Discount");
            if (discounts == null || !discounts.Any())
            {
                ModelState.AddModelError(string.Empty, "Không tìm thấy mã giảm giá nào.");
                return View();
            }

            // Passing the data to the View
            ViewBag.Brands = new SelectList(brands, "Id", "BrandName");
            ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");

            ViewBag.Discounts = discounts.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = $"{d.Percent}%"  // Thêm ký tự phần trăm
            }).ToList();

            return View(product);
        }

    }
}

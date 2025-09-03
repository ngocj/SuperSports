using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SP.Application.Dto.BrandDto;
using SP.Application.Dto.CategoryDto;
using SP.Application.Dto.DiscountDto;
using SP.Application.Dto.ProductDto;
using SP.Application.Dto.ProductVariantDto;
using SP.Application.Dto.UserDto;
using SP.Domain.Entity;
using System.Globalization;
using System.Reflection.Metadata;
using System;
using System.Text.Json;
using static Azure.Core.HttpHeader;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SP.WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private const string ApiUrl = "https://localhost:7131/api/product";
        private const string ApiUrl1 = "https://localhost:7131/api/";
        private HttpClient _httpClient;
        public ProductController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        
        public async Task<ActionResult> Details(int id)
        {
            var response = await _httpClient.GetFromJsonAsync<ProductViewDto>($"{ApiUrl}/{id}");
            if (response == null)
            {
                return NotFound();
            }
            return View(response);
        }
        public async Task<IActionResult> Create()
        {
            var brands = await _httpClient.GetFromJsonAsync<IEnumerable<BrandViewDto>>($"{ApiUrl1}brand");
            if (brands == null || !brands.Any())
            {
                ModelState.AddModelError(string.Empty, "Không tìm thấy thương hiệu nào.");
                return View();
            }

            var categories = await _httpClient.GetFromJsonAsync<IEnumerable<CategoryViewDto>>($"{ApiUrl1}category");
            if (categories == null || !categories.Any())
            {
                ModelState.AddModelError(string.Empty, "Không tìm thấy danh mục nào");
                return View();
            }

            var subCategories = await _httpClient.GetFromJsonAsync<IEnumerable<SubCategoryViewDto>>($"{ApiUrl1}subcategory");
            if (subCategories == null || !subCategories.Any())
            {
                ModelState.AddModelError(string.Empty, "Không tìm thấy danh mục con nào");
                return View();
            }

            var discounts = await _httpClient.GetFromJsonAsync<IEnumerable<DiscountViewDto>>($"{ApiUrl1}Discount");
            if (discounts == null || !discounts.Any())
            {
                ModelState.AddModelError(string.Empty, "Không tìm thấy mã giảm giá nào.");
                return View();
            }

            ViewBag.Brands = new SelectList(brands, "Id", "BrandName");
            ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");
            ViewBag.CategoriesJson = System.Text.Json.JsonSerializer.Serialize(categories);
            ViewBag.SubCategoriesJson = System.Text.Json.JsonSerializer.Serialize(subCategories); // <-- Fix quan trọng

            ViewBag.Discounts = discounts.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = $"{d.Percent}%"
            }).ToList();

            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Create([FromForm] ProductCreateDto productCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return View(productCreateDto);
            }

            var response = await _httpClient.PostAsJsonAsync(ApiUrl, productCreateDto);

            if (response.IsSuccessStatusCode)
            {
                // Đọc phản hồi trả về object có Id
                var content = await response.Content.ReadFromJsonAsync<ProductViewDto>();

                if (content?.Id != null)
                {
                    return RedirectToAction("CreateProductVariant", "ProductVariant", new { ProductId = content.Id });
                }

                ModelState.AddModelError("", "Không thể lấy ProductId từ phản hồi.");
                return View(productCreateDto);
            }
            // tempdata thông báo lỗi
            TempData["Error"] = "Tên sản phẩm đã tồn tại.";     
            return View(productCreateDto);
        }
    
        public async Task<ActionResult> Edit(int id)
        {
            // get all brands
            var brands = await _httpClient.GetFromJsonAsync<IEnumerable<BrandViewDto>>($"{ApiUrl1}brand");
            if (brands == null || !brands.Any())
            {
                ModelState.AddModelError(string.Empty, "No brands found.");
                return View();
            }

            // get all subcategories
            var subCategories = await _httpClient.GetFromJsonAsync<IEnumerable<SubCategoryViewDto>>($"{ApiUrl1}subcategory");
            if (subCategories == null || !subCategories.Any())
            {
                ModelState.AddModelError(string.Empty, "No subcategories found.");
                return View();
            }

            // get all discounts
            var discounts = await _httpClient.GetFromJsonAsync<IEnumerable<DiscountViewDto>>($"{ApiUrl1}Discount");
            if (discounts == null || !discounts.Any())
            {
                ModelState.AddModelError(string.Empty, "No discounts found.");
                return View();
            }
            var response = await _httpClient.GetFromJsonAsync<ProductUpdateDto>($"{ApiUrl}/{id}");
            if (response == null)
            {
                return NotFound();
            }

            // FIX: Truyền selected value
            ViewBag.Brands = new SelectList(brands, "Id", "BrandName", response.BrandId);
            ViewBag.Categories = new SelectList(subCategories, "Id", "Name", response.SubCategoryId);
            ViewBag.Discounts = discounts.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = $"{d.Percent}%",
                Selected = (d.Id == response.DiscountId) // Discount dùng SelectListItem nên selected xử lý riêng
            }).ToList();
 
            return View(response);

        }
        [HttpPost]
        public async Task<ActionResult> Edit(ProductUpdateDto productUpdate)
        {
            if (!ModelState.IsValid)
            {
                return View(productUpdate);
            }
            var response = await _httpClient.PutAsJsonAsync(ApiUrl, productUpdate);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Cập nhật sản phẩm thành công!";
            }
            else
            {
                TempData["Error"] = "Cập nhật sản phẩm thất bại.";
            }
            return RedirectToAction("GetAllProduct", "Admin");
          
        }
        public async Task<ActionResult> Delete(int id)
        {
            // Kiểm tra xem sản phẩm có tồn tại không
            var product = await _httpClient.GetFromJsonAsync<ProductViewDto>($"{ApiUrl}/{id}");
            if (product == null)
            {
                return NotFound();
            }

            // Gửi yêu cầu DELETE đến API
            var response = await _httpClient.DeleteAsync($"{ApiUrl}/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Xóa sản phẩm thành công!";              
            }
            else
            {
                TempData["Error"] = "Không thể xóa vì sản phẩm đang có đơn hàng.";
            }
            return RedirectToAction("GetAllProduct", "Admin");
        }
       

    }
}

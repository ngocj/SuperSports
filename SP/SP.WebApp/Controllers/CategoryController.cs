using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SP.Application.Dto.CategoryDto;

namespace SP.WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private const string ApiUrl = "https://localhost:7131/api/category";
        private HttpClient _httpClient;

        public CategoryController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public IActionResult CreateCategory()
        {
           return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateCategory(CategoryCreateDto categoryCreateDto)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiUrl, categoryCreateDto);
           
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Thêm danh mục thành công.";
            }
            else
            {
                TempData["Error"] = "Tên danh mục đã tồn tại.";
            }
            return RedirectToAction("GetAllCategory", "Admin");
        }
        public async Task<ActionResult> UpdateCategory(int id)
        {
            var categoryViewDto = await _httpClient.GetFromJsonAsync<CategoryUpdateDto>($"{ApiUrl}/{id}");
            return View(categoryViewDto);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateCategory(CategoryUpdateDto categoryUpdateDto)
        {
            var response = await _httpClient.PutAsJsonAsync(ApiUrl, categoryUpdateDto);           
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Cập nhật danh mục thành công.";
            }
            else
            {
                TempData["Error"] = "Tên danh mục đã tồn tại.";
            }
            return RedirectToAction("GetAllCategory", "Admin");
        }
        public async Task<ActionResult> DeleteCategory(int id)
        {
            var response = await _httpClient.DeleteAsync($"{ApiUrl}/{id}");           
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Xóa danh mục thành công.";
            }
            else
            {
                TempData["Error"] = "Xóa danh mục không thành công.";
            }
            return RedirectToAction("GetAllCategory", "Admin");
           
        }
        public async Task<ActionResult> DetailCategory(int id)
        {
            var categoryViewDto = await _httpClient.GetFromJsonAsync<CategoryViewDto>($"{ApiUrl}/{id}");
            if (categoryViewDto == null)
            {
                return NotFound();
            }
            return View(categoryViewDto);
        }
    }
}

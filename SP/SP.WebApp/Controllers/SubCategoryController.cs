using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SP.Application.Dto.CategoryDto;

namespace SP.WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SubCategoryController : Controller
    {
        private const string ApiUrl = "https://localhost:7131/api/subcategory";
        private HttpClient _httpClient;

        public SubCategoryController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        [HttpGet]
        public IActionResult CreateSubCategory(int categoryId)
        {
            var model = new SubCategoryCreateDto
            {
                CategoryId = categoryId
            };
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateSubCategory(SubCategoryCreateDto subCategoryCreateDto)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiUrl, subCategoryCreateDto);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Thêm danh mục con thành công.";
            }
            else
            {
                TempData["Error"] = "Tên danh mục con đã tồn tại.";
            }
            return RedirectToAction("DetailCategory", "Category", new {Id = subCategoryCreateDto.CategoryId });
        }

        public async Task<ActionResult> UpdateSubCategory(int id)
        {
            var subCategoryViewDto = await _httpClient.GetFromJsonAsync<SubCategoryViewDto>($"{ApiUrl}/{id}");
            return View(subCategoryViewDto);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateSubCategory(SubCategoryViewDto subCategoryUpdateDto)
        {
            var response = await _httpClient.PutAsJsonAsync(ApiUrl, subCategoryUpdateDto);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Cập nhật danh mục con thành công.";
            }
            else
            {
                TempData["Error"] = "Tên danh mục con đã tồn tại.";
            }
            return RedirectToAction("DetailCategory", "Category", new { Id = subCategoryUpdateDto.CategoryId });
        }

        public async Task<ActionResult> DeleteSubCategory(int id)
        {
            var subCategoryViewDto = await _httpClient.GetFromJsonAsync<SubCategoryViewDto>($"{ApiUrl}/{id}");
            if (subCategoryViewDto == null)
            {
                return NotFound();
            }
            var response = await _httpClient.DeleteAsync($"{ApiUrl}/{id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Xóa danh mục con thành công.";
            }
            else
            {
                TempData["Error"] = "Xóa danh mục con không thành công.";
            }
            return RedirectToAction("DetailCategory", "Category", new { Id = subCategoryViewDto.CategoryId });
        }

        
    }
}

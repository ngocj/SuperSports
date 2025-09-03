using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SP.Application.Dto.BrandDto;

namespace SP.WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BrandController : Controller
    {
        private const string ApiUrl = "https://localhost:7131/api/brand";
        private HttpClient _httpClient;

        public BrandController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CreateBrand()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateBrand(BrandCreateDto brandCreateDto)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiUrl, brandCreateDto);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Thêm thương hiệu thành công.";
            }
            else
            {
                TempData["Error"] = "Tên thương hiệu đã tồn tại.";
            }
            return RedirectToAction("GetAllBrand", "Admin");
        }
        public async Task<ActionResult> UpdateBrand(int id)
        {
            var brandViewDto = await _httpClient.GetFromJsonAsync<BrandViewDto>($"{ApiUrl}/{id}");
            return View(brandViewDto);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateBrand(BrandViewDto brandUpdateDto)
        {
            var response = await _httpClient.PutAsJsonAsync(ApiUrl, brandUpdateDto);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Cập nhật thương hiệu thành công.";
            }
            else
            {
                TempData["Error"] = "Tên thương hiệu đã tồn tại.";
            }
            return RedirectToAction("GetAllBrand", "Admin");
        }
        public async Task<ActionResult> DeleteBrand(int id)
        {
            var response = await _httpClient.DeleteAsync($"{ApiUrl}/{id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Xóa thương hiệu thành công.";
            }
            else
            {
                TempData["Error"] = "Xóa thương hiệu không thành công.";
            }
            return RedirectToAction("GetAllBrand", "Admin");
        }


       
        
    }
}

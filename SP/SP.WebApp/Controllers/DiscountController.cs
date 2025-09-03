using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SP.Application.Dto.DiscountDto;

namespace SP.WebApp.Controllers
{
    [Authorize(Roles = "Manager")]
    public class DiscountController : Controller
    {
        private const string ApiUrl = "https://localhost:7131/api/discount";
        private HttpClient _httpClient;

        public DiscountController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public IActionResult CreateDiscount()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateDiscount(DiscountCreateDto discountCreateDto)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiUrl, discountCreateDto);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Thêm mã giảm giá thành công.";
            }
            else
            {
                TempData["Error"] = "Tên mã giảm giá đã tồn tại.";
            }
            return RedirectToAction("GetAllDiscount", "Manager");
        }

        public async Task<ActionResult> UpdateDiscount(int id)
        {
            var discountViewDto = await _httpClient.GetFromJsonAsync<DiscountViewDto>($"{ApiUrl}/{id}");
            return View(discountViewDto);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateDiscount(DiscountViewDto discountUpdateDto)
        {
            var response = await _httpClient.PutAsJsonAsync(ApiUrl, discountUpdateDto);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Cập nhật mã giảm giá thành công.";
            }
            else
            {
                TempData["Error"] = "Tên mã giảm giá đã tồn tại.";
            }
            return RedirectToAction("GetAllDiscount", "Manager");
        }

        public async Task<ActionResult> DeleteDiscount(int id)
        {
            var response = await _httpClient.DeleteAsync($"{ApiUrl}/{id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Xóa mã giảm giá thành công.";
            }
            else
            {
                TempData["Error"] = "Xóa mã giảm giá không thành công.";
            }
            return RedirectToAction("GetAllDiscount", "Manager");
        }

        
    }
}

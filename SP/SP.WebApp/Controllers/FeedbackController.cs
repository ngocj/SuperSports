using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SP.Application.Dto.FeedbackDto;
using SP.Application.Dto.OrderDetailDto;
using SP.Domain.Entity;
using System.IdentityModel.Tokens.Jwt;

namespace SP.WebApp.Controllers
{
    
    public class FeedbackController : Controller
    {
        private const string ApiUrl = "https://localhost:7131/api/feedback";
        private const string ApiUrl1 = "https://localhost:7131/api/";
        private HttpClient _httpClient;

        public FeedbackController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteFeedback(int id)
        {
            var response = await _httpClient.DeleteAsync($"{ApiUrl}/{id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Xóa đánh giá  thành công.";
            }
            else
            {
                TempData["Error"] = "Xóa đánh giá không thành công.";
            }
            return RedirectToAction("GetAllFeedback", "Admin");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DetailFeedback(int id)
        {
            var feedback = await _httpClient.GetFromJsonAsync<FeedbackViewDto>($"{ApiUrl}/{id}");
            if (feedback == null)
            {
                return NotFound();
            }
            return View(feedback);
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateFeedback(Guid orderId, int productVariantId)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrWhiteSpace(token))
                return RedirectToAction("Login", "Auth");

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid");
            var userNameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name");

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                return RedirectToAction("Login", "Auth");

            var orderDetail = await _httpClient.GetFromJsonAsync<OrderDetailViewDto>(
                $"{ApiUrl1}orderdetail/{orderId}/{productVariantId}");

            if (orderDetail == null)
            {
                TempData["Error"] = "Không tìm thấy chi tiết đơn hàng.";
                return RedirectToAction("OrderHistory", "Order");
            }

            var dto = new FeedbackCreateDto
            {
                OrderId = orderId,
                ProductVariantId = productVariantId,
                UserId = userId,
                OrderDetail = orderDetail 
            };


            return View(dto);
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateFeedback(FeedbackCreateDto feedbackCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return View(feedbackCreateDto);
            }

            var response = await _httpClient.PostAsJsonAsync(ApiUrl, feedbackCreateDto);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Thêm đánh giá thành công.";
                return RedirectToAction("OrderHistory", "Order");
            }

            TempData["Error"] = "Thêm đánh giá không thành công.";
            return View(feedbackCreateDto);
        }

    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SP.Application.Dto.ImageDto;

namespace SP.WebApp.Controllers
{
    [Authorize(Roles ="Admin")]
    public class ImageController : Controller
    {
        private const string ApiUrl = "https://localhost:7131/api/ImageFile";
        private readonly HttpClient _httpClient;

        public ImageController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        [HttpPost]
        public async Task<ActionResult> DeleteImage(int id)
        {
            var response = await _httpClient.DeleteAsync($"{ApiUrl}/{id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Xóa hình ảnh thành công.";            
            }
            else
            {
                TempData["Error"] = "Xóa hình ảnh không thành công.";
            }
            return RedirectToAction("GetAllProductVariant", "ProductVariant");
        
        }
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file, int productVariantId)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Vui lòng chọn một tệp ảnh hợp lệ.";
                return RedirectToAction("GetAllProductVariant", "ProductVariant");
            }

            using var content = new MultipartFormDataContent();

            // Stream file
            var streamContent = new StreamContent(file.OpenReadStream());
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

            // Thêm file với đúng tên field: "file" (viết thường để trùng với tên bên API)
            content.Add(streamContent, "file", file.FileName);

            // Thêm productVariantId (viết đúng tên khớp tham số trong API)
            content.Add(new StringContent(productVariantId.ToString()), "productVariantId");

            var response = await _httpClient.PostAsync($"{ApiUrl}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Tải ảnh lên thành công.";
            }
            else
            {
                TempData["Error"] = "Hình ảnh đã tồn tại.";
            }

            return RedirectToAction("GetAllProductVariant", "ProductVariant");
        }

    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SP.Application.Dto.ProductVariantDto;
using System.Net.Http;

namespace SP.WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductVariantController : Controller
    {
        private const string ApiUrl = "https://localhost:7131/api/productVariant";
        private const string ApiUrl1 = "https://localhost:7131/api/";
        private readonly HttpClient _httpClient;
        public ProductVariantController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        public IActionResult CreateProductVariant(int ProductId)
        {
            var model = new VariantCreateDto
            {
                ProductId = ProductId
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateProductVariant([FromForm] VariantCreateDto variantCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return View(variantCreateDto);
            }

            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(variantCreateDto.ProductId.ToString()), "ProductId");
            content.Add(new StringContent(variantCreateDto.Color), "Color");
            content.Add(new StringContent(variantCreateDto.Size), "Size");
            content.Add(new StringContent(variantCreateDto.Price.ToString()), "Price");
            content.Add(new StringContent(variantCreateDto.Quantity.ToString()), "Quantity");
            content.Add(new StringContent(variantCreateDto.IsActive.ToString()), "IsActive");

            if (variantCreateDto.Images != null && variantCreateDto.Images.Count > 0)
            {
                foreach (var file in variantCreateDto.Images)
                {
                    var streamContent = new StreamContent(file.OpenReadStream());
                    streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                    content.Add(streamContent, "Images", file.FileName);
                }
            }

            var response = await _httpClient.PostAsync(ApiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Tạo biến thể sản phẩm thành công.";
                return RedirectToAction("Details", "Product", new { Id = variantCreateDto.ProductId });

            }
            else
            {
                TempData["Error"] = "(Kích cỡ màu sắc) hoặc ảnh đã tồn tại .";
                return View(variantCreateDto);
            }

        }
        public async Task<IActionResult> DeleteProductVariant(int id)
        {
            var variantResponse = await _httpClient.GetFromJsonAsync<VariantViewDto>($"{ApiUrl}/{id}");
            if (variantResponse == null)
            {
                return NotFound();
            }

            var productId = variantResponse.ProductId;
            var response = await _httpClient.DeleteAsync($"{ApiUrl}/{id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Xóa biến thể sản phẩm thành công.";
                return RedirectToAction("Details", "Product", new {Id = variantResponse.ProductId });
            }
            else
            {
                TempData["Error"] = "Xóa biến thể sản phẩm thất bại.";
                return RedirectToAction("Details", "Product", new { Id = variantResponse.ProductId });

            }
        }
        public async Task<IActionResult> EditProductVariant(int id)
        {
            var response = await _httpClient.GetFromJsonAsync<VariantViewDto>($"{ApiUrl}/{id}");
            if (response == null)
            {
                return NotFound();
            }


            return View(response);
        }
        [HttpPost]
        public async Task<IActionResult> EditProductVariant([FromForm] VariantUpdateDto variantUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return View(variantUpdateDto);
            }

            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(variantUpdateDto.Id.ToString()), "Id"); // nhớ thêm Id
            content.Add(new StringContent(variantUpdateDto.ProductId.ToString()), "ProductId");
            content.Add(new StringContent(variantUpdateDto.Color), "Color");
            content.Add(new StringContent(variantUpdateDto.Size), "Size");
            content.Add(new StringContent(variantUpdateDto.Price.ToString()), "Price");
            content.Add(new StringContent(variantUpdateDto.Quantity.ToString()), "Quantity");
            content.Add(new StringContent(variantUpdateDto.IsActive.ToString()), "IsActive");

            // Thêm ảnh mới (nếu có)
            if (variantUpdateDto.Images != null && variantUpdateDto.Images.Count > 0)
            {
                foreach (var file in variantUpdateDto.Images)
                {
                    var streamContent = new StreamContent(file.OpenReadStream());
                    streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                    content.Add(streamContent, "Images", file.FileName);
                }
            }

            var response = await _httpClient.PutAsync(ApiUrl, content); // PUT với multipart

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Cập nhật biến thể sản phẩm thành công.";
                return RedirectToAction("Details", "Product", new { Id = variantUpdateDto.ProductId });
            }
            else
            {
                TempData["Error"] = "(Kích cỡ màu sắc) hoặc ảnh đã tồn tại .";
                return View(variantUpdateDto);
            }
        }

        public async Task<IActionResult> GetAllProductVariant()
        {
            var response = await _httpClient.GetFromJsonAsync<List<VariantViewDto>>(ApiUrl);
            if (response == null)
            {
                return NotFound();
            }
            return View(response);
        }
   
    }
}

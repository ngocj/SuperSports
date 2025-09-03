using Microsoft.AspNetCore.Mvc;
using SP.Application.Dto.ProductDto;
using SP.Application.Dto.ProductVariantDto;
using System.Net.Http;
using System.Net.Http.Json;

namespace SP.WebApp.Controllers
{
    [Route("[controller]")]
    public class ChatBotGeminiController : Controller
    {
        private readonly IChatbotGeminiService _chatbotGeminiService;

        public ChatBotGeminiController(IChatbotGeminiService chatbotGeminiService)
        {
            _chatbotGeminiService = chatbotGeminiService;
        }

        [HttpPost("SendMessageGemini")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessageGemini([FromBody] ChatMessage message)
        {
            if (string.IsNullOrWhiteSpace(message?.Text))
            {
                return Json(new { success = false, reply = "Vui lòng nhập nội dung câu hỏi." });
            }

            try
            {
                var reply = await _chatbotGeminiService.AskGeminiAsync(message.Text);

                if (string.IsNullOrEmpty(reply))

                {
                    return Json(new { success = false, reply = "Không nhận được phản hồi từ AI. Vui lòng thử lại." });
                }

                return Json(new { success = true, reply });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendMessageGemini: {ex}");
                return Json(new { success = false, reply = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }
    }

    public class ChatMessage
    {
        public string Text { get; set; }
    }

    public interface IChatbotGeminiService
    {
        Task<string> AskGeminiAsync(string userMessage);
    }

    public class ChatbotGeminiService : IChatbotGeminiService
    {
        private readonly HttpClient _httpClient;
        private const string _apiKey = ""; // TODO: Move to appsettings //AIzaSyAr3O1EViTQIAUDG-szlEClO1SFokGG_r8
        private const string GeminiEndpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";
        private const string ApiUrl = "https://localhost:7131/api/product";
        private const string ApiUrl1 = "https://localhost:7131/api/productVariant";
        public ChatbotGeminiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> AskGeminiAsync(string userMessage)
        {
            var products = await _httpClient.GetFromJsonAsync<List<ProductViewDto>>(ApiUrl);
            var variants = await _httpClient.GetFromJsonAsync<List<VariantViewDto>>(ApiUrl1);

            if (products == null || variants == null || !products.Any())
            {
                return "Không tìm thấy sản phẩm nào để tư vấn.";
            }

            // Dùng markdown để tạo link có thể ấn vào
            string productListString = string.Join("\n", products
                .Where(p => p.IsActive)
                .Select(p =>
                {
                    var relatedVariants = variants.Where(v => v.ProductId == p.Id && v.IsActive);
                    var minPrice = relatedVariants.Min(v => (decimal?)v.Price) ?? 0;

                    return $"- **{p.ProductName}** ({minPrice:N0} VND) - [Xem chi tiết](https://localhost:7179/Home/Detail/{p.Id})";
                }));

            string promptIntro =
                "Bạn là trợ lý tư vấn mua sắm thông minh của SuperSports. " +
                "Dưới đây là danh sách sản phẩm hiện có:\n\n" + productListString + "\n\n" +
                "Hãy tư vấn cho khách hàng dựa trên câu hỏi của họ. " +
                "Khi tư vấn sản phẩm, hãy tuân theo các nguyên tắc sau:\n\n" +

                "4. CHÍNH SÁCH MUA HÀNG:\n" +
                "- Đổi trả miễn phí trong 30 ngày\n" +
                "- Bảo hành chính hãng\n" +
                "- Giao hàng toàn quốc\n" +
                "- Hỗ trợ trả góp 0%\n\n" +
                "Thông tin liên hệ:\n" +
                "- Website: [SuperSports](https://localhost:7179)\n" +
                "- Hotline: [1900.1234](tel:19001234)\n" +
                "- Showroom: [Số 1 Đại Cồ Việt, Hai Bà Trưng, Hà Nội](https://maps.google.com/?q=1+Dai+Co+Viet+Hai+Ba+Trung+Ha+Noi)\n\n" +
                "Khi trả lời:\n" +
                "1. Luôn tư vấn chi tiết và chuyên nghiệp\n" +
                "2. Cung cấp link sản phẩm cụ thể để khách hàng dễ dàng tham khảo\n" +
                "3. Nhấn mạnh chính sách bảo hành và đổi trả\n" +
                "4. Đề xuất các sản phẩm liên quan phù hợp\n" +
                "5. Kết hợp tư vấn cả về kỹ thuật và thẩm mỹ";

            string fullPrompt = $"{promptIntro}\n\nKhách hàng hỏi: \"{userMessage}\"\n\nHãy trả lời ngắn gọn, súc tích, không quá 3 câu.";

            var requestBody = new
            {
                contents = new[]
                {
        new
        {
            parts = new[]
            {
                new { text = fullPrompt }
            }
        }
    },
                generationConfig = new
                {
                    maxOutputTokens = 500,
                    temperature = 0.5
                }
            };


            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{GeminiEndpoint}?key={_apiKey}", requestBody);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Gemini API Error: {errorContent}");
                    return $"Xin lỗi, tôi gặp sự cố khi trả lời. Vui lòng thử lại sau. (Mã lỗi: {response.StatusCode})";
                }

                var result = await response.Content.ReadFromJsonAsync<GeminiResponse>();
                var replyText = result?.candidates?.FirstOrDefault()?.content?.parts?.FirstOrDefault()?.text?.Trim();

                return string.IsNullOrEmpty(replyText)
                    ? "Xin lỗi, tôi không thể xử lý yêu cầu này ngay bây giờ. Vui lòng thử lại sau."
                    : replyText;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in AskGeminiAsync: {ex}");
                return $"Xin lỗi, có lỗi xảy ra: {ex.Message}";
            }
        }



        public class GeminiResponse
        {
            public List<Candidate> candidates { get; set; }
        }

        public class Candidate
        {
            public Content content { get; set; }
        }

        public class Content
        {
            public List<Part> parts { get; set; }
        }

        public class Part
        {
            public string text { get; set; }
        }
    }
}

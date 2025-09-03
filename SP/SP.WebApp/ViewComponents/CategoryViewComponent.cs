using Microsoft.AspNetCore.Mvc;
using SP.Application.Dto.CategoryDto;

namespace SP.WebApp.ViewComponents
{
    [ViewComponent(Name = "Category")]
    public class CategoryViewComponent : ViewComponent
    {
        private const string ApiUrl = "https://localhost:7131/api/category";
        private readonly HttpClient _httpClient;

        public CategoryViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<CategoryViewDto>>(ApiUrl);
            return View(result);
        }
    }
} 

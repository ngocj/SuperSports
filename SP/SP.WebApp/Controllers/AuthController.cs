using Microsoft.AspNetCore.Mvc;
using SP.Application.Dto.LoginDto;
using SP.Application.Dto.UserDto;
using System.Linq;

namespace SP.WebApp.Controllers
{
    public class AuthController : Controller
    {
        private const string ApiUrl = "https://localhost:7131/api/auth";
        private const string ApiUrlUser = "https://localhost:7131/api/user";
        private HttpClient _httpClient;

        public AuthController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewDto loginViewDto)
        {
            if (!ModelState.IsValid)
            {
                return View(loginViewDto);
            }

            var response = await _httpClient.PostAsJsonAsync(ApiUrl, loginViewDto);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "❌ Email hoặc mật khẩu không đúng.");
                return View(loginViewDto);
            }

            var token = await response.Content.ReadAsStringAsync();
            HttpContext.Session.SetString("JwtToken", token);
            Response.Cookies.Append("Jwt", token ?? "", new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax
            });

            return RedirectToAction("Index", "Home");
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }
 
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("JwtToken");
            try
            {
                Response.Cookies.Delete("Jwt");
            }
            catch { }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto userViewDto)
        {
            if (!ModelState.IsValid)
            {
                return View(userViewDto);
            }

            var response = await _httpClient.PostAsJsonAsync($"{ApiUrl}/register", userViewDto);

            // check email already exists
            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                ModelState.AddModelError("", "❌ Email đã tồn tại.");
                return View(userViewDto);
            }         
            // check phone number already exists
            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                ModelState.AddModelError("", "❌ Số điện thoại đã tồn tại.");
                return View(userViewDto);
            }

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Đã xảy ra lỗi khi tạo tài khoản.";
                return View(userViewDto);
            }
        
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Tài khoản đã được tạo thành công!";


            }

            return RedirectToAction("Login", "Auth");
        }
    }
}

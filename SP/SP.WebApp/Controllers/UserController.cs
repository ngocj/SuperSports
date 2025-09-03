using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SP.Application.Dto.CategoryDto;
using SP.Application.Dto.EmployeeDto;
using SP.Application.Dto.UserDto;
using SP.WebApp.Models;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text.Json;

namespace SP.WebApp.Controllers
{
    
    public class UserController : Controller
    {
        private const string ApiUrl = "https://localhost:7131/api/user";
        private const string ApiUrl2 = "https://localhost:7131/api/employee";
        private const string ApiUrl1 = "https://localhost:7131/api/";
        private readonly HttpClient _httpClient;

        public UserController(IHttpClientFactory httpClient)
        {

            _httpClient = httpClient.CreateClient();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult CreateUser()
        {
            ViewBag.Roles = new List<SelectListItem>
            {
                new SelectListItem { Text = "Quản trị viên", Value = "1" },
                new SelectListItem { Text = "Quản lý", Value = "2" },
                new SelectListItem { Text = "Khách hàng", Value = "4" }
            };
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser(UserCreateDto userCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return View(userCreateDto);
            }

            var response = await _httpClient.PostAsJsonAsync($"{ApiUrl}", userCreateDto);

            if (!response.IsSuccessStatusCode)
            {
                // Đọc nội dung lỗi từ response
                var content = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    try
                    {
                        // Deserialize JSON lỗi từ API: { "field": "Email", "message": "Email đã tồn tại." }
                        var errorObj = JsonSerializer.Deserialize<Dictionary<string, string>>(content);

                        if (errorObj != null && errorObj.ContainsKey("field") && errorObj.ContainsKey("message"))
                        {
                            ModelState.AddModelError(errorObj["field"], $"❌ {errorObj["message"]}");
                        }
                        else
                        {
                            ModelState.AddModelError("", "❌ Lỗi không xác định.");
                        }
                    }
                    catch
                    {
                        ModelState.AddModelError("", "❌ Không thể đọc lỗi từ server.");
                    }

                    return View(userCreateDto);
                }

                TempData["Error"] = "❌ Thêm người dùng thất bại.";
                return View(userCreateDto);
            }

            TempData["Success"] = "🎉 Thêm người dùng thành công!";
            return RedirectToAction("GetAllUser", "Admin");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(Guid id)
        {
            ViewBag.Roles = new List<SelectListItem>
            {
                new SelectListItem { Text = "Quản trị viên", Value = "1" },
                new SelectListItem { Text = "Quản lý", Value = "2" },
                new SelectListItem { Text = "Khách hàng", Value = "4" }
            };
            var response = await _httpClient.GetFromJsonAsync<UserUpdateDto>($"{ApiUrl}/{id}");
            return View(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(UserUpdateDto userUpdateDto)
        {
            // Lấy thông tin người dùng hiện tại từ API bằng ID
            var existingUser = await _httpClient.GetFromJsonAsync<UserUpdateDto>($"{ApiUrl}/{userUpdateDto.Id}");

            if (existingUser == null)
            {
                TempData["Error"] = "Không tìm thấy người dùng.";
                return RedirectToAction("GetAllUser", "Admin");
            }       

            // Gửi yêu cầu cập nhật
            var response = await _httpClient.PutAsJsonAsync(ApiUrl, userUpdateDto);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Cập nhật người dùng thành công!";
            }
            else
            {
                TempData["Error"] = "Cập nhật người dùng thất bại.";
            }

            return RedirectToAction("GetAllUser", "Admin");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {         
            var response = await _httpClient.DeleteAsync($"{ApiUrl}/{id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Xóa người dùng thành công!";
            }
            else
            {
                TempData["Error"] = "Xóa người dùng thất bại.";
            }
            return RedirectToAction("GetAllUser", "Admin");          
        }

        [Authorize(Roles = "User,Admin,Manager")]
        public async Task<IActionResult> ProfileUser(Guid id)
        {
            var categories = await _httpClient.GetFromJsonAsync<IEnumerable<CategoryViewDto>>($"{ApiUrl1}category");
            ViewBag.Categories = categories != null ? new SelectList(categories, "Id", "CategoryName") : null;
            try
            {
                // Check authentication and get user roles
                var token = HttpContext.Session.GetString("JwtToken");
                var isLoggedIn = !string.IsNullOrEmpty(token);

                var userRoles = new List<string>();
                if (isLoggedIn)
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(token);
                    userRoles = jwtToken.Claims
                        .Where(c => c.Type == "role")
                        .Select(c => c.Value)
                        .ToList();
                }

                // Set role flags in ViewBag
                ViewBag.IsAdmin = userRoles.Contains("Admin");
                ViewBag.IsManager = userRoles.Contains("Manager");
                ViewBag.IsUser = userRoles.Contains("User");

                // Get user data
                var response = await _httpClient.GetAsync($"{ApiUrl}/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    // Handle API errors appropriately
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return NotFound();
                    }

                    // Log error and return appropriate view
                    return View("Error", new ErrorViewModel
                    {
                        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                    });
                }

                var user = await response.Content.ReadFromJsonAsync<UserViewDto>();
                return View(user);
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> ProfileEmployee(Guid id)
        {
            var categories = await _httpClient.GetFromJsonAsync<IEnumerable<CategoryViewDto>>($"{ApiUrl1}category");
            ViewBag.Categories = categories != null ? new SelectList(categories, "Id", "CategoryName") : null;
            try
            {
                // Check authentication and get user roles
                var token = HttpContext.Session.GetString("JwtToken");
                var isLoggedIn = !string.IsNullOrEmpty(token);

                var userRoles = new List<string>();
                if (isLoggedIn)
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(token);
                    userRoles = jwtToken.Claims
                        .Where(c => c.Type == "role")
                        .Select(c => c.Value)
                        .ToList();
                }
                ViewBag.IsEmployee = userRoles.Contains("Employee");
                // Get user data
                var response = await _httpClient.GetAsync($"{ApiUrl2}/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    // Handle API errors appropriately
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return NotFound();
                    }

                    // Log error and return appropriate view
                    return View("Error", new ErrorViewModel
                    {
                        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                    });
                }

                var user = await response.Content.ReadFromJsonAsync<EmployeeViewDto>();
                return View(user);
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DetailUser(Guid id)
        {     
            var response = await _httpClient.GetFromJsonAsync<UserViewDto>($"{ApiUrl}/{id}");
            return View(response);
        }

    }
}

using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Generators;
using SP.Application.Dto.LoginDto;
using SP.Domain.Entity;
using SP.Infrastructure.Context;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SP.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly SPContext _context;
        private readonly IMapper _mapper;
        public AuthController(IMapper mapper, IConfiguration config, SPContext context)
        {
            _mapper = mapper;
            _config = config;
            _context = context;
        }



        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginViewDto loginViewDto)
        {
            if (loginViewDto == null || string.IsNullOrEmpty(loginViewDto.Email) || string.IsNullOrEmpty(loginViewDto.Password))
            {
                return BadRequest("Email and password cannot be blank.");
            }

            // Tìm theo email
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == loginViewDto.Email);
            var employee = await _context.Employees.FirstOrDefaultAsync(x => x.Email == loginViewDto.Email);

            // Kiểm tra tài khoản tồn tại và active
            if (user == null && employee == null)
            {
                return Unauthorized("Incorrect email or password.");
            }

            if (user != null && user.IsActive == false)
            {
                return Unauthorized("User account is not active.");
            }

            if (employee != null && employee.IsActive == false)
            {
                return Unauthorized("Employee account is not active.");
            }

            // Kiểm tra mật khẩu
            bool isUserValid = user != null && BCrypt.Net.BCrypt.Verify(loginViewDto.Password, user.Password);
            bool isEmployeeValid = employee != null && BCrypt.Net.BCrypt.Verify(loginViewDto.Password, employee.Password);

            if (!isUserValid && !isEmployeeValid)
            {
                return Unauthorized("Incorrect email or password.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

            ClaimsIdentity identity;

            if (isEmployeeValid)
            {
                identity = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.NameIdentifier, employee.Id.ToString()),
            new Claim(ClaimTypes.Name, employee.Name ?? "Employee"),
            new Claim(ClaimTypes.Email, employee.Email),
            new Claim(ClaimTypes.Role, "Employee")
                });
            }
            else // isUserValid
            {
                var roleName = await _context.Roles
                    .Where(x => x.Id == user.RoleId)
                    .Select(x => x.RoleName)
                    .FirstOrDefaultAsync() ?? "User";

                identity = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, roleName)
                });
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(tokenString);
        }




        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerViewDto)
        {
            if (registerViewDto == null || string.IsNullOrEmpty(registerViewDto.Email) || string.IsNullOrEmpty(registerViewDto.Password))
            {
                return BadRequest("Email and password cannot be blank.");
            }

            var userExists = _context.Users.Any(x => x.Email == registerViewDto.Email);
            if (userExists)
            {
                return Conflict("Email already exists.");
            }

            var phoneExists = _context.Users.Any(x => x.PhoneNumber == registerViewDto.PhoneNumber);
            if (phoneExists)
            {
                return Conflict("Phone number already exists.");
            }

            var user = _mapper.Map<User>(registerViewDto);

            // 🔐 Hash mật khẩu trước khi lưu
            user.Password = BCrypt.Net.BCrypt.HashPassword(registerViewDto.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync(); // Đừng quên lưu thay đổi

            return Ok("User registered successfully.");
        }
    }
}

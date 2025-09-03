using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SP.Application.Dto.UserDto;
using SP.Application.Service.Interface;
using SP.Domain.Entity;
using SP.Infrastructure.Context;

namespace SP.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly SPContext _context;

        public UserController(IMapper mapper, IUserService userService, SPContext context)
        {
            _mapper = mapper;
            _userService = userService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            var userDto = _mapper.Map<IEnumerable<UserViewDto>>(users);
            return Ok(userDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            var userDto = _mapper.Map<UserViewDto>(user);
            return Ok(userDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto userCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Kiểm tra email trùng
                if (await _context.Users.AnyAsync(u => u.Email == userCreateDto.Email))
                {
                    return Conflict(new { field = "Email", message = "Email đã tồn tại." });
                }

                // Kiểm tra số điện thoại trùng
                if (await _context.Users.AnyAsync(u => u.PhoneNumber == userCreateDto.PhoneNumber))
                {
                    return Conflict(new { field = "PhoneNumber", message = "Số điện thoại đã tồn tại." });
                }

                var user = _mapper.Map<User>(userCreateDto);

                await _userService.CreateUser(user);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi tạo người dùng.", detail = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDto userUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.GetUserById(userUpdateDto.Id);
            if (user == null)
            {
                return NotFound();
            }

            // Map DTO vào object đã truy xuất từ DB
            _mapper.Map(userUpdateDto, user);

            // Gọi service để lưu
            await _userService.UpdateUser(user);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            await _userService.DeleteUser(id);
            return NoContent();
        }


    }
}

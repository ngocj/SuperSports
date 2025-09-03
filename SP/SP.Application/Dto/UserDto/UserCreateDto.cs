using System.ComponentModel.DataAnnotations;

namespace SP.Application.Dto.UserDto
{
    public class UserCreateDto
    {    

        public string UserName { get; set; }

        public string Email { get; set; }

        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu không khớp")]
        public string ConfirmPassword { get; set; }


        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải gồm đúng 10 chữ số.")]
        public string? PhoneNumber { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public string? AddressDetail { get; set; }
        public bool IsActive { get; set; }
        public int RoleId { get; set; }

    }
}

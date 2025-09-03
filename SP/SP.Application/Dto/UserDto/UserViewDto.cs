using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Application.Dto.UserDto
{
    public class UserViewDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? AddressDetail { get; set; }
        public bool? IsActive { get; set; }
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
        public int? WardId { get; set; }
      
    }
}

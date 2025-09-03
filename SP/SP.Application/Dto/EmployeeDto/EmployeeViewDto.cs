using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Application.Dto.EmployeeDto
{
    public class EmployeeViewDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
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

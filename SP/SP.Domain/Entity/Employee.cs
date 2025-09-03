using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Domain.Entity
{
    public class Employee
    {
        public Guid Id { get; set; }
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? PhoneNumber { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? AddressDetail { get; set; }
        public bool? IsActive { get; set; }
        public int? WardId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Ward? Ward { get; set; }
        public Role Role { get; set; }
        public List<Order> Orders { get; set; }

    }
}

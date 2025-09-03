using SP.Application.Dto.OrderDetailDto;
using SP.Application.Dto.UserDto;
using SP.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Application.Dto.OrderDto
{
    public class OrderCreateDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public UserViewDto? User { get; set; }
        public Guid? EmployeeId { get; set; }
        public OrderStatus Status { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public decimal TotalPrice { get; set; }
        public string? UserName { get; set; }
        public string? EmployeeName { get; set; }
        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }
        public int? WardId { get; set; }
        public string? Note { get; set; }
        public string? PhoneNumber { get; set; }
        public string? AddressDetail { get; set; }
        public string? ProductName { get; set; }
        public List<OrderDetailCreateDto> OrderDetails { get; set; }
    }
}

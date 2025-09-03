using SP.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Application.Dto.OrderDto
{
    public class OrderUpdateDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid? EmployeeId { get; set; }
        public OrderStatus Status { get; set; }
    }
}

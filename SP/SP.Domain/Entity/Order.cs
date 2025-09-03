using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Domain.Entity
{
    public enum OrderStatus
    {
        Pending,      // Chờ xác nhận
        Confirmed,    // Đã xác nhận
        Shipping,     // Đang giao
        Paid,         // Đã thanh toán
        Delivered,    // Đã giao
        Canceled      // Đã huỷ
    }
    public enum PaymentMethod
    {
        Cash,
        VnPay,
    }
    public class Order 
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid? EmployeeId { get; set; }
        public OrderStatus Status { get; set; }
        public PaymentMethod PaymentMethod { get; set; } 
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public User User { get; set; }
        public Employee Employee { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }   

    }
}

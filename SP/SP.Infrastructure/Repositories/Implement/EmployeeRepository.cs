using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SP.Domain.Entity;
using SP.Infrastructure.Context;
using SP.Infrastructure.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Infrastructure.Repositories.Implement
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(SPContext context) : base(context)
        {
        }
        public override async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _SPContext.Set<Employee>()
                .Include(e => e.Role)
                .ToListAsync();
        }
        public override async Task<Employee> GetByIdAsync(Guid id)
        {
            return await _SPContext.Set<Employee>()
                .Include(e => e.Role)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<string>> GetCustomerNamesHandledByAsync(Guid employeeId)
        {
            return await _SPContext.Orders
               .Where(o => o.EmployeeId == employeeId)
               .Select(o => o.User.UserName)
               .Distinct()
               .ToListAsync();
        }

        public async Task<int> GetHandledOrderCountAsync(Guid employeeId)
        {
            return await _SPContext.Orders
                .CountAsync(o => o.EmployeeId == employeeId && o.Status == OrderStatus.Delivered);
        }

        public  async Task<decimal> GetRevenueByEmployeeAsync(Guid employeeId)
        {
            return await _SPContext.Orders
               .Where(o => o.EmployeeId == employeeId)
               .SumAsync(o => o.TotalPrice);
        }
        public class HandledOrderDto
        {
            public Guid OrderId { get; set; }
            public string CustomerName { get; set; }
            public DateTime OrderDate { get; set; }
            public decimal TotalPrice { get; set; }

            public PaymentMethod PaymentMethod { get; set; }
            public OrderStatus Status { get; set; }
        }

        public async Task<IEnumerable<HandledOrderDto>> GetHandledOrdersByEmployeeAsync(Guid employeeId)
        {
            return await _SPContext.Orders
                .Where(o => o.EmployeeId == employeeId && o.Status == OrderStatus.Delivered)
                .Select(o => new HandledOrderDto
                {
                    OrderId = o.Id,
                    CustomerName = o.User.UserName,
                    OrderDate = o.CreatedAt,
                    TotalPrice = o.TotalPrice,
                    PaymentMethod = o.PaymentMethod,
                    Status = o.Status

                })
                .ToListAsync();
        }



        public class EmployeeStatsDto
        {
            public Guid EmployeeId { get; set; }
            public string EmployeeName { get; set; }
            public int HandledOrderCount { get; set; }
            public decimal TotalRevenue { get; set; }
            public int CustomerCount { get; set; }
            public double CompletionRate { get; set; } // 👈 mới
        }

        public async Task<IEnumerable<EmployeeStatsDto>> GetAllEmployeeStatisticsAsync()
        {
            var employees = await _SPContext.Employees
                .Include(e => e.Role)
                .ToListAsync();

            var result = new List<EmployeeStatsDto>();

            foreach (var emp in employees)
            {
                // Tất cả đơn hàng của nhân viên
                var allOrders = await _SPContext.Orders
                    .Where(o => o.EmployeeId == emp.Id)
                    .Include(o => o.OrderDetails)
                    .ToListAsync();

                // Đơn đã giao thành công
                var deliveredOrders = allOrders
                    .Where(o => o.Status == OrderStatus.Delivered)
                    .ToList();

                var totalOrders = allOrders.Count;
                var handledCount = deliveredOrders.Count;
                var revenue = deliveredOrders.Sum(o => o.OrderDetails.Sum(od => od.Quantity * od.Price));
                var customerCount = deliveredOrders.Select(o => o.UserId).Distinct().Count();


                result.Add(new EmployeeStatsDto
                {
                    EmployeeId = emp.Id,
                    EmployeeName = emp.Name,
                    HandledOrderCount = handledCount,
                    TotalRevenue = revenue,
                    CustomerCount = customerCount,
                });
            }

            return result;
        }


    }

}

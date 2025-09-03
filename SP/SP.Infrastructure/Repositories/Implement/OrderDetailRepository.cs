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
    public class OrderDetailRepository : GenericRepository<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(SPContext context) : base(context)
        {

        }

        public override async Task<OrderDetail> GetByCompositeKeyAsync(Guid id1, int id2)
        {
            return await _SPContext.OrderDetails
                .Include(od => od.ProductVariant)
                    .ThenInclude(pv => pv.Product)
                .Include(od => od.ProductVariant)
                    .ThenInclude(pv => pv.Images)
                .Include(od => od.FeedBacks) // Include Feedback
                .FirstOrDefaultAsync(od => od.OrderId == id1 && od.ProductVariantId == id2);
        }

        public override async Task<IEnumerable<OrderDetail>> GetAllAsync()
        {
            return await _SPContext.OrderDetails
                .Include(od => od.ProductVariant)
                    .ThenInclude(pv => pv.Product)
                .Include(od => od.ProductVariant)
                    .ThenInclude(pv => pv.Images)
                .Include(od => od.FeedBacks) // Include Feedback
                .ToListAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _SPContext.OrderDetails
                .Where(od => od.Order.Status == OrderStatus.Delivered)
                .SumAsync(od => od.Price * od.Quantity);
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime from, DateTime to)
        {
            var toInclusive = to.Date.AddDays(1);

            return await _SPContext.OrderDetails
                .Where(od => od.Order.Status == OrderStatus.Delivered)
                .Where(od => od.Order.CreatedAt >= from && od.Order.CreatedAt < toInclusive)
                .SumAsync(od => od.Price * od.Quantity);
        }

        public async Task<int> GetCompletedOrderCountAsync()
        {
            return await _SPContext.Orders
                .CountAsync(o => o.Status == OrderStatus.Delivered);
        }

        public async Task<int> GetTotalProductPendingAsync()
        {
            return await _SPContext.OrderDetails
                .CountAsync(od => od.Order.Status == OrderStatus.Pending);
        }
        public async Task<int> GetTotalProductDeliveredAsync()
        {
            return await _SPContext.OrderDetails
                .CountAsync(od => od.Order.Status == OrderStatus.Delivered);
        }

        public async Task<int> GetTotalProductCanceledAsync()
        {
            return await _SPContext.OrderDetails
                .CountAsync(od => od.Order.Status == OrderStatus.Canceled);
        }

        public async Task<int> GetTotalProductShippingAsync()
        {
            return await _SPContext.OrderDetails
                .CountAsync(od => od.Order.Status == OrderStatus.Shipping);
        }

        public async Task<int> GetTotalProductSoldAsync()
        {
            return await _SPContext.OrderDetails
                .SumAsync(od => od.Quantity);
        }

        public async Task<IEnumerable<TopSellingVariant>> GetTopSellingVariantsAsync(int top = 5)
        {
            if (top <= 0)
                throw new ArgumentException("Top phải lớn hơn 0.");

            var orderDetails = await _SPContext.OrderDetails
                .Include(od => od.ProductVariant)
                    .ThenInclude(pv => pv.Product)
                .ToListAsync();

            return orderDetails
                .GroupBy(od => od.ProductVariantId)
                .OrderByDescending(g => g.Sum(od => od.Quantity))
                .Take(top)
                .Select(g =>
                {
                    var first = g.FirstOrDefault();
                    var variant = first?.ProductVariant;

                    return new TopSellingVariant
                    {
                        ProductVariantId = g.Key,
                        Quantity = g.Sum(od => od.Quantity),
                        Name = variant?.Product?.ProductName ?? "Không xác định",
                        Size = variant?.Size ?? "Không xác định",
                        Color = variant?.Color ?? "Không xác định"
                    };
                })
                .ToList();
        }

        public async Task<int> GetTotalOrderCountAsync()
        {
            return await _SPContext.Orders.CountAsync();
        }

        public async Task<decimal> GetAverageOrderValueAsync()
        {
            var completedOrders = await _SPContext.Orders
                .Where(o => o.Status == OrderStatus.Delivered)
                .Select(o => o.TotalPrice)
                .ToListAsync();

            return completedOrders.Any() 
                ? completedOrders.Average(p => p) 
                : 0;
        }

        public async Task<int> GetCanceledOrderCountAsync()
        {
            return await _SPContext.Orders
                .CountAsync(o => o.Status == OrderStatus.Canceled);
        }

        public async Task<IEnumerable<TopCustomer>> GetTopCustomersAsync(int count = 5)
        {
            return await _SPContext.Orders
                .Where(o => o.Status == OrderStatus.Delivered)
                .GroupBy(o => new { o.UserId, o.User.UserName })
                .Select(g => new TopCustomer
                {
                    UserId = g.Key.UserId,
                    Name = g.Key.UserName,
                    OrderCount = g.Count(),
                    TotalSpent = g.Sum(o => o.TotalPrice)
                })
                .OrderByDescending(c => c.TotalSpent)
                .Take(count)
                .ToListAsync();
        }

        public async Task<RevenueData> GetRevenueByPeriodAsync(string period)
        {
            var today = DateTime.Today;
            var data = new RevenueData { Labels = new List<string>(), Values = new List<decimal>() };

            switch (period.ToLower())
            {
                case "month":
                    // Doanh thu 30 ngày gần nhất
                    var last30Days = Enumerable.Range(0, 30)
                        .Select(i => today.AddDays(-i))
                        .Reverse()
                        .ToList();

                    foreach (var date in last30Days)
                    {
                        var revenue = await _SPContext.Orders
                            .Where(o => o.Status == OrderStatus.Delivered &&
                                   o.CreatedAt.Date == date.Date)
                            .SumAsync(o => o.TotalPrice);

                        data.Labels.Add(date.ToString("dd/MM"));
                        data.Values.Add(revenue);
                    }
                    break;

                case "quarter":
                    // Doanh thu 3 tháng gần nhất
                    var last3Months = Enumerable.Range(0, 3)
                        .Select(i => new DateTime(today.Year, today.Month, 1).AddMonths(-i))
                        .Reverse()
                        .ToList();

                    foreach (var month in last3Months)
                    {
                        var revenue = await _SPContext.Orders
                            .Where(o => o.Status == OrderStatus.Delivered &&
                                   o.CreatedAt.Year == month.Year &&
                                   o.CreatedAt.Month == month.Month)
                            .SumAsync(o => o.TotalPrice);

                        data.Labels.Add(month.ToString("MM/yyyy"));
                        data.Values.Add(revenue);
                    }
                    break;

                case "year":
                    // Doanh thu 12 tháng gần nhất
                    var last12Months = Enumerable.Range(0, 12)
                        .Select(i => new DateTime(today.Year, today.Month, 1).AddMonths(-i))
                        .Reverse()
                        .ToList();

                    foreach (var month in last12Months)
                    {
                        var revenue = await _SPContext.Orders
                            .Where(o => o.Status == OrderStatus.Delivered &&
                                   o.CreatedAt.Year == month.Year &&
                                   o.CreatedAt.Month == month.Month)
                            .SumAsync(o => o.TotalPrice);

                        data.Labels.Add(month.ToString("MM/yyyy"));
                        data.Values.Add(revenue);
                    }
                    break;
            }

            return data;
        }

        public class TopSellingVariant
        {
            public int ProductVariantId { get; set; }
            public int Quantity { get; set; }
            public string Name { get; set; }
            public string Size { get; set; }
            public string Color { get; set; }
        }

        public class TopCustomer
        {
            public Guid UserId { get; set; }
            public string Name { get; set; }
            public int OrderCount { get; set; }
            public decimal TotalSpent { get; set; }
        }

        public class RevenueData
        {
            public List<string> Labels { get; set; }
            public List<decimal> Values { get; set; }
        }
    }
}

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
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(SPContext context) : base(context)
        {

        }

        public async Task<bool> CancelOrderAsync(Guid orderId)
        {
            var order = await _SPContext.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProductVariant)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null || order.Status == OrderStatus.Canceled)
                return false;

            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Confirmed)
                return false;


            order.Status = OrderStatus.Canceled;
            order.UpdatedAt = DateTime.UtcNow;

            foreach (var item in order.OrderDetails)
            {
                var variant = item.ProductVariant;
                if (variant != null)
                {
                    variant.Quantity += item.Quantity;
                }
            }

            // Đánh dấu thay đổi và lưu vào DB
            _SPContext.Update(order);
            await _SPContext.SaveChangesAsync();

            return true;
        }


        public override async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _SPContext.Set<Order>()
                .Include(fb => fb.User)
                .Include(fb => fb.Employee)
                .Include(fb => fb.OrderDetails)
                    .ThenInclude(od => od.ProductVariant)
                    .ThenInclude(pv => pv.Product)
                .ToListAsync();
        }

        public override async Task<Order> GetByIdAsync(Guid id)
        {
            var order = await _SPContext.Set<Order>()
                .Include(fb => fb.User)
                .Include(fb => fb.Employee)
                .Include(fb => fb.OrderDetails)
                    .ThenInclude(od => od.ProductVariant)
                        .ThenInclude(pv => pv.Images)
                .Include(fb => fb.OrderDetails)
                    .ThenInclude(od => od.ProductVariant)
                        .ThenInclude(pv => pv.Product)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (order != null)
            {
                // Nếu bạn lưu giá tại thời điểm đặt hàng trong OrderDetail.Price
                order.TotalPrice = order.OrderDetails.Sum(od => od.ProductVariant.Price * od.Quantity);

                // Nếu bạn muốn dùng giá hiện tại trong ProductVariant (ít khuyến khích)
                // order.TotalPrice = order.OrderDetails.Sum(od => od.ProductVariant.Price * od.Quantity);
            }

            return order;
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(Guid userId)
        {
            return await _SPContext.Set<Order>()
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProductVariant)
                        .ThenInclude(pv => pv.Product)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.ProductVariant)
                        .ThenInclude(pv => pv.Images)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }


    }

}

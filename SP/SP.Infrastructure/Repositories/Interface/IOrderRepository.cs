using SP.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Infrastructure.Repositories.Interface
{
    public interface IOrderRepository : IGenericRepository<Order>
    {

        Task<List<Order>> GetOrdersByUserIdAsync(Guid userId);
        Task<bool> CancelOrderAsync(Guid orderId);

    }
}

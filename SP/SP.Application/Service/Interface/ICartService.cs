using SP.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Application.Service.Interface
{
    public interface ICartService 
    {
        Task<IEnumerable<Cart>> GetAllCarts();
        Task<Cart> GetCartById(Guid userId, int productVariantId);
        Task CreateCart(Cart cart);
        Task UpdateCart(Cart cart);
        Task DeleteCart(Guid userId, int productVariantId);
        Task<List<Cart>> GetAllCartsByUserIdAsync(Guid userId);

    }
}

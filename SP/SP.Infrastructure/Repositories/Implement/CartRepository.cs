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
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        public CartRepository(SPContext context) : base(context)
        {
        }
        public override async Task<IEnumerable<Cart>> GetAllAsync()
        {
            return await _SPContext.Set<Cart>()
           .Include(c => c.ProductVariant)
               .ThenInclude(pv => pv.Images)
           .Include(c => c.ProductVariant)
               .ThenInclude(pv => pv.Product)
           .ToListAsync();
        }
        public override async Task<Cart> GetByCompositeKeyAsync (Guid userId, int productVariantId)
        {
            return await _SPContext.Set<Cart>()
                .Include(c => c.ProductVariant)
                    .ThenInclude(pv => pv.Images)
                .Include(c => c.ProductVariant)
                    .ThenInclude(pv => pv.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductVariantId == productVariantId);
        }
     

        public async Task<List<Cart>> GetAllCartsByUserIdAsync(Guid userId)
        {
            return await _SPContext.Set<Cart>()
                .Include(c => c.ProductVariant)
                    .ThenInclude(pv => pv.Images)
                .Include(c => c.ProductVariant)
                    .ThenInclude(pv => pv.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }
    }

}

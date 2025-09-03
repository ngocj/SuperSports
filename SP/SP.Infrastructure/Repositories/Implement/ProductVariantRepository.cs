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
    public class ProductVariantRepository : GenericRepository<ProductVariant>, IProductVariantRepository
    {
        public ProductVariantRepository(SPContext context) : base(context)
        {
        }
        public async override Task<IEnumerable<ProductVariant>> GetAllAsync()
        {
            // include image
            return  await _SPContext.ProductVariants
                .Include(p => p.Product)
                .Include(im => im.Images).ToListAsync();

        }
        public async override Task<ProductVariant> GetByIdAsync(int id)
        {
            return await _SPContext.ProductVariants
               .Include(im => im.Images)
               .FirstOrDefaultAsync(p => p.Id == id);
        }
     
    }
    
}

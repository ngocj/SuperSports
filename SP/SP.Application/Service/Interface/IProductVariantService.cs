using SP.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Application.Service.Interface
{
    public interface IProductVariantService
    {
        Task<IEnumerable<ProductVariant>> GetAllProductVariants();
        Task<ProductVariant> GetProductVariantById(int id);
        Task CreateProductVariant(ProductVariant productVariant);
        Task UpdateProductVariant(ProductVariant productVariant);
        Task DeleteProductVariant(int id);

    }
}

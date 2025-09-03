using SP.Domain.Entity;
using SP.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SP.Infrastructure.Repositories.Implement.ProductRepository;

namespace SP.Infrastructure.Repositories.Interface
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> GetAllByBrandIdAsync(int brandId);
        Task<IEnumerable<Product>> GetAllByCategoryIdAsync(int categoryId);
        Task<IEnumerable<Product>> GetAllBySubCategoryIdAsync(int subCategoryId);
        Task<IEnumerable<Product>> GetAllByCategoryAndBrandAsync(int? SubcategoryId, int? brandId, bool? isActive);
        Task<IEnumerable<Product>> GetAllByLastestAsync(decimal? priceFrom, decimal? priceTo, int categoryId, int? subCategoryId, int? brandId,string? search);
        Task<IEnumerable<Product>> GetAllByBestSellingAsync(decimal? priceFrom, decimal? priceTo, int categoryId, int? subCategoryId, int? brandId, string? search);
        Task<IEnumerable<Product>> GetAllByPriceAscendingAsync(decimal? priceFrom, decimal? priceTo, int categoryId, int? subCategoryId, int? brandId, string? search);
        Task<IEnumerable<Product>> GetAllByPriceDescendingAsync(decimal? priceFrom, decimal? priceTo, int categoryId, int? subCategoryId, int? brandId, string? search);
        Task<IEnumerable<Product>> GetTop10BestSellingAsync();
        Task<IEnumerable<Product>> GetTop10NewestAsync();

        // Thống kê số lượng sản phẩm theo từng danh mục, bao gồm tên danh mục
        Task<List<ProductCountByCategoryDto>> GetProductCountByCategoryWithNamesAsync();

        // Thống kê số lượng sản phẩm theo từng thương hiệu, bao gồm tên thương hiệu
        Task<List<ProductCountByBrandDto>> GetProductCountByBrandWithNamesAsync();

        /// Thống kê sản phẩm sắp hết hàng (số lượng tồn kho thấp), kèm theo thông tin chi tiết
        Task<IEnumerable<LowStockProductDto>> GetLowStockProductDetailsAsync(int threshold = 10);

        // Thống kê doanh thu theo sản phẩm (dựa trên tổng giá trị bán), bao gồm tên sản phẩm và tên danh mục
        Task<IEnumerable<TopRevenueProductDto>> GetTopRevenueProductDetailsAsync(int topCount = 10);
    }
}

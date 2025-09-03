using SP.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SP.Infrastructure.Repositories.Implement.ProductRepository;

namespace SP.Application.Service.Interface
{
    public interface IProductService
    {
        Task DeleteProduct(int id);
        Task CreateProduct(Product product);
        Task UpdateProduct(Product product);
        Task<Product> GetProductById(int id);
        Task<IEnumerable<Product>> GetAllProducts();
        Task<IEnumerable<Product>> GetAllProductsByBrandId(int brandId);
        Task<IEnumerable<Product>> GetAllByCategoryIdAsync(int categoryId);
        Task<IEnumerable<Product>> GetAllProductsBySubCategoryId(int subCategoryId);
        Task<IEnumerable<Product>> GetAllByCategoryAndBrandAsync(int? SubcategoryId, int? brandId, bool? isActive);
        Task<IEnumerable<Product>> GetAllByLastestAsync(decimal? priceFrom, decimal? priceTo, int categoryId, int? subCategoryId, int? brandId, string? search);
        Task<IEnumerable<Product>> GetAllProductsByBestSelling(decimal? priceFrom, decimal? priceTo, int categoryId, int? subCategoryId, int? brandId, string? search);
        Task<IEnumerable<Product>> GetAllProductsByPriceAscending(decimal? priceFrom, decimal? priceTo, int categoryId, int? subCategoryId, int? brandId, string? search);
        Task<IEnumerable<Product>> GetAllProductsByPriceDescending(decimal? priceFrom, decimal? priceTo, int categoryId, int? subCategoryId, int? brandId, string? search);
        Task<IEnumerable<Product>> GetTop10BestSellingAsync();
        Task<IEnumerable<Product>> GetTop10NewestAsync();

        Task<List<ProductCountByCategoryDto>> GetProductCountByCategoryWithNamesAsync();

        // Thống kê số lượng sản phẩm theo từng thương hiệu, bao gồm tên thương hiệu
        Task<List<ProductCountByBrandDto>> GetProductCountByBrandWithNamesAsync();

        /// Thống kê sản phẩm sắp hết hàng (số lượng tồn kho thấp), kèm theo thông tin chi tiết
        Task<IEnumerable<LowStockProductDto>> GetLowStockProductDetailsAsync(int threshold = 10);

        // Thống kê doanh thu theo sản phẩm (dựa trên tổng giá trị bán), bao gồm tên sản phẩm và tên danh mục
        Task<IEnumerable<TopRevenueProductDto>> GetTopRevenueProductDetailsAsync(int topCount = 10);
    }
}

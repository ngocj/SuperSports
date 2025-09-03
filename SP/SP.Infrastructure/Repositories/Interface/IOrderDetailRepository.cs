using SP.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SP.Infrastructure.Repositories.Implement.OrderDetailRepository;

namespace SP.Infrastructure.Repositories.Interface
{
    public interface IOrderDetailRepository : IGenericRepository<OrderDetail>
    {
        // Tổng doanh thu toàn hệ thống
        Task<decimal> GetTotalRevenueAsync();

        // Tổng doanh thu theo khoảng thời gian
        Task<decimal> GetTotalRevenueAsync(DateTime from, DateTime to);

        // Số đơn hàng đã hoàn thành
        Task<int> GetCompletedOrderCountAsync();

        // Số sản phẩm đã bán
        Task<int> GetTotalProductSoldAsync();

        // Thống kê top sản phẩm bán chạy nhất
        Task<IEnumerable<TopSellingVariant>> GetTopSellingVariantsAsync(int top = 5);

        // Thống kê tổng số sản phẩm đã giao
        Task<int> GetTotalProductDeliveredAsync();

        // Thống kê tổng số sản phẩm đã hủy
        Task<int> GetTotalProductCanceledAsync();

        // Thống kê tổng số sản phẩm đang giao
        Task<int> GetTotalProductShippingAsync();

        // Thống kê tổng số sản phẩm đang chờ
        Task<int> GetTotalProductPendingAsync();

        // Tổng số đơn hàng
        Task<int> GetTotalOrderCountAsync();

        // Giá trị đơn hàng trung bình
        Task<decimal> GetAverageOrderValueAsync();

        // Số đơn hàng đã hủy
        Task<int> GetCanceledOrderCountAsync();

        // Top khách hàng chi tiêu nhiều
        Task<IEnumerable<TopCustomer>> GetTopCustomersAsync(int count = 5);

        // Doanh thu theo khoảng thời gian (tháng/quý/năm)
        Task<RevenueData> GetRevenueByPeriodAsync(string period);

        // thong ke danh sach doanh so nhan vien cho manager



    }
}

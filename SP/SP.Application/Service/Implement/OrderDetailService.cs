using SP.Application.Service.Interface;
using SP.Domain.Entity;
using SP.Infrastructure.Repositories.Implement;
using SP.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SP.Application.Service.Implement
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderDetailService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<OrderDetail>> GetAllOrderDetails()
        {
            return await _unitOfWork.OrderDetailRepository.GetAllAsync();
        }

        public async Task<OrderDetail> GetOrderDetailById(Guid orderId, int productVariantId)
        {
            return await _unitOfWork.OrderDetailRepository.GetByCompositeKeyAsync(orderId, productVariantId);
        }

        public async Task CreateOrderDetail(OrderDetail orderDetail)
        {
            await _unitOfWork.OrderDetailRepository.AddAsync(orderDetail);
            await _unitOfWork.SaveChangeAsync();
        }

        public async Task UpdateOrderDetail(OrderDetail orderDetail)
        {
            var result = await _unitOfWork.OrderDetailRepository.GetByCompositeKeyAsync(orderDetail.OrderId, orderDetail.ProductVariantId);
            if (result != null)
            {
                await _unitOfWork.OrderDetailRepository.UpdateAsync(orderDetail);
                await _unitOfWork.SaveChangeAsync();
            }
        }

        public async Task DeleteOrderDetail(Guid orderId, int productVariantId)
        {
            var result = await _unitOfWork.OrderDetailRepository.GetByCompositeKeyAsync(orderId, productVariantId);
            if (result != null)
            {
                await _unitOfWork.OrderDetailRepository.DeleteAsync(result);
                await _unitOfWork.SaveChangeAsync();
            }
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _unitOfWork.OrderDetailRepository.GetTotalRevenueAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime from, DateTime to)
        {
            return await _unitOfWork.OrderDetailRepository.GetTotalRevenueAsync(from, to);
        }

        public async Task<int> GetCompletedOrderCountAsync()
        {
            return await _unitOfWork.OrderDetailRepository.GetCompletedOrderCountAsync();
        }

        public async Task<int> GetTotalProductSoldAsync()
        {
            return await _unitOfWork.OrderDetailRepository.GetTotalProductSoldAsync();
        }

        public async Task<IEnumerable<OrderDetailRepository.TopSellingVariant>> GetTopSellingVariantsAsync(int top = 5)
        {
            return await _unitOfWork.OrderDetailRepository.GetTopSellingVariantsAsync(top);
        }

        public async Task<int> GetTotalProductDeliveredAsync()
        {
            return await _unitOfWork.OrderDetailRepository.GetTotalProductDeliveredAsync();
        }

        public async Task<int> GetTotalProductCanceledAsync()
        {
            return await _unitOfWork.OrderDetailRepository.GetTotalProductCanceledAsync();
        }

        public async Task<int> GetTotalProductShippingAsync()
        {
            return await _unitOfWork.OrderDetailRepository.GetTotalProductShippingAsync();
        }

        public async Task<int> GetTotalProductPendingAsync()
        {
            return await _unitOfWork.OrderDetailRepository.GetTotalProductPendingAsync();
        }

        public async Task<int> GetTotalOrderCountAsync()
        {
            return await _unitOfWork.OrderDetailRepository.GetTotalOrderCountAsync();
        }   

        public async Task<decimal> GetAverageOrderValueAsync()
        {
            return await _unitOfWork.OrderDetailRepository.GetAverageOrderValueAsync();
        }

        public async Task<int> GetCanceledOrderCountAsync()
        {
            return await _unitOfWork.OrderDetailRepository.GetCanceledOrderCountAsync();
        }

        public async Task<IEnumerable<OrderDetailRepository.TopCustomer>> GetTopCustomersAsync(int count = 5)
        {
            return await _unitOfWork.OrderDetailRepository.GetTopCustomersAsync(count);
        }

        public async Task<OrderDetailRepository.RevenueData> GetRevenueByPeriodAsync(string period)
        {
            return await _unitOfWork.OrderDetailRepository.GetRevenueByPeriodAsync(period);
        }
    }
}

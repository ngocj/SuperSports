using SP.Application.Service.Interface;
using SP.Domain.Entity;
using SP.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Application.Service.Implement
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CancelOrderAsync(Guid orderId)
        {
           return await _unitOfWork.OrderRepository.CancelOrderAsync(orderId);
        }

        public  async Task CreateOrder(Order order)
        {
            await _unitOfWork.OrderRepository.AddAsync(order);
            await _unitOfWork.SaveChangeAsync();

            
        }

        public async Task DeleteOrder(Guid id)
        {
            var result = await _unitOfWork.OrderRepository.GetByIdAsync(id);
            if (result != null)
            {
                await _unitOfWork.OrderRepository.DeleteAsync(result);
                await _unitOfWork.SaveChangeAsync();
            }

            
        }
        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            return await _unitOfWork.OrderRepository.GetAllAsync();
            
        }

        public async Task<Order> GetOrderById(Guid id)
        {
            return await _unitOfWork.OrderRepository.GetByIdAsync(id);
            
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(Guid userId)
        {

            return await _unitOfWork.OrderRepository.GetOrdersByUserIdAsync(userId);
        }

        public async Task UpdateOrder(Order order)
        {
            var result = await _unitOfWork.OrderRepository.GetByIdAsync(order.Id);
            if (result != null)
            {
                await _unitOfWork.OrderRepository.UpdateAsync(order);
                await _unitOfWork.SaveChangeAsync();
            }
            
        }
    }
}

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
    public class DiscountService : IDiscountService
    {
        private readonly IUnitOfWork _unitOfWork;
        public DiscountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task CreateDiscount(Discount discount)
        {
            await _unitOfWork.DiscountRepository.AddAsync(discount);
            await _unitOfWork.SaveChangeAsync();
          
        }

        public async Task DeleteDiscount(int id)
        {
            var result = await _unitOfWork.DiscountRepository.GetByIdAsync(id);
            if (result != null)
            {
                await _unitOfWork.DiscountRepository.DeleteAsync(result);
                await _unitOfWork.SaveChangeAsync();
            }
            
        }

        public Task<IEnumerable<Discount>> GetAllDiscounts()
        {
            return _unitOfWork.DiscountRepository.GetAllAsync();
            
        }

        public Task<Discount> GetDiscountById(int id)
        {
            return _unitOfWork.DiscountRepository.GetByIdAsync(id);
            
        }

        public async Task UpdateDiscount(Discount discount)
        {
            var result = await _unitOfWork.DiscountRepository.GetByIdAsync(discount.Id);
            if (result != null)
            {
                await _unitOfWork.DiscountRepository.UpdateAsync(discount);
                await _unitOfWork.SaveChangeAsync();
            }
            
        }
    }
}

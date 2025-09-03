using MailKit.Search;
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
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateCart(Cart cart)
        {
            await _unitOfWork.CartRepository.AddAsync(cart);
            await _unitOfWork.SaveChangeAsync();
        }

        public async Task DeleteCart(Guid userId, int productVariantId)
        {
            var result = await _unitOfWork.CartRepository.GetByCompositeKeyAsync(userId, productVariantId);
            if (result != null)
            {
                await _unitOfWork.CartRepository.DeleteAsync(result);
                await _unitOfWork.SaveChangeAsync();
            }
        }

        public async Task<IEnumerable<Cart>> GetAllCarts()
        {
            return await _unitOfWork.CartRepository.GetAllAsync();
        }

        public async Task<Cart> GetCartById(Guid userId, int productVariantId)
        {
            return await _unitOfWork.CartRepository.GetByCompositeKeyAsync(userId, productVariantId);
        }

        public async Task UpdateCart(Cart cart)
        {
            var existingCart = await _unitOfWork.CartRepository.GetByCompositeKeyAsync(cart.UserId, cart.ProductVariantId);

            if (existingCart != null)
            {
                await _unitOfWork.CartRepository.UpdateAsync(cart);
                await _unitOfWork.SaveChangeAsync();
            }
        }
        public async Task<List<Cart>> GetAllCartsByUserIdAsync(Guid userId)
        {
            return await _unitOfWork.CartRepository.GetAllCartsByUserIdAsync(userId);
        }
    }

}

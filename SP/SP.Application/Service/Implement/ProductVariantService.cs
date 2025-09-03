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
    public class ProductVariantService : IProductVariantService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductVariantService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task CreateProductVariant(ProductVariant productVariant)
        {
            await _unitOfWork.ProductVariantRepository.AddAsync(productVariant);
            await _unitOfWork.SaveChangeAsync();

            
        }

        public async Task DeleteProductVariant(int id)
        {
            var result = await _unitOfWork.ProductVariantRepository.GetByIdAsync(id);
            if (result != null)
            {
                await _unitOfWork.ProductVariantRepository.DeleteAsync(result);
                await _unitOfWork.SaveChangeAsync();
            }
            
        }

        public async Task<IEnumerable<ProductVariant>> GetAllProductVariants()
        {
            return await  _unitOfWork.ProductVariantRepository.GetAllAsync();
            
        }

        public async Task<ProductVariant> GetProductVariantById(int id)
        {
            return await _unitOfWork.ProductVariantRepository.GetByIdAsync(id);
            
        }

        public async Task UpdateProductVariant(ProductVariant productVariant)
        {
            var result = await _unitOfWork.ProductVariantRepository.GetByIdAsync(productVariant.Id);
            if (result != null)
            {
                await _unitOfWork.ProductVariantRepository.UpdateAsync(productVariant);
                await _unitOfWork.SaveChangeAsync();
            }
            
        }
    }
}

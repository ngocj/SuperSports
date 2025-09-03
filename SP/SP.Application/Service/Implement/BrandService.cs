using Microsoft.AspNetCore.Http;
using SP.Application.Service.Interface;
using SP.Domain.Entity;
using SP.Infrastructure.UnitOfWork;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SP.Application.Service.Implement
{
    public class BrandService : IBrandService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BrandService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<Brand>> GetAllBrands()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            var roles = user?.Claims
                .Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
                .Select(c => c.Value)
                .ToList();

            var allBrands = await _unitOfWork.BrandRepository.GetAllAsync();

            if (roles != null && (roles.Contains("Admin")))
            {
                return allBrands; // Trả về tất cả
            }

            // Trả về chỉ brand active
            return allBrands.Where(b => b.IsActive);
        }

        public async Task<Brand> GetBrandById(int id)
        {
            return await _unitOfWork.BrandRepository.GetByIdAsync(id);
        }

        public async Task CreateBrand(Brand brand)
        {
            await _unitOfWork.BrandRepository.AddAsync(brand);
            await _unitOfWork.SaveChangeAsync();
        }

        public async Task UpdateBrand(Brand brand)
        {
            var result = await _unitOfWork.BrandRepository.GetByIdAsync(brand.Id);
            if (result != null)
            {
                await _unitOfWork.BrandRepository.UpdateAsync(brand);
                await _unitOfWork.SaveChangeAsync();
            }
        }

        public async Task DeleteBrand(int id)
        {
            var result = await _unitOfWork.BrandRepository.GetByIdAsync(id);
            if (result != null)
            {
                await _unitOfWork.BrandRepository.DeleteAsync(result);
                await _unitOfWork.SaveChangeAsync();
            }
        }
    }
}

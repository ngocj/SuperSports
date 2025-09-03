using Microsoft.AspNetCore.Http;
using SP.Application.Service.Interface;
using SP.Domain.Entity;
using SP.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SP.Application.Service.Implement
{
    public class SubCategoryService : ISubCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SubCategoryService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task CreateSubCategory(SubCategory subCategory)
        {
                
            await _unitOfWork.SubCategoryRepository.AddAsync(subCategory);
            await _unitOfWork.SaveChangeAsync();
           
        }

        public async Task DeleteSubCategory(int id)
        {
        
            var result = await _unitOfWork.SubCategoryRepository.GetByIdAsync(id);
            if (result != null)
            {
                await _unitOfWork.SubCategoryRepository.DeleteAsync(result);
                await _unitOfWork.SaveChangeAsync();
            }
        }

        public async Task<IEnumerable<SubCategory>> GetAllSubCategories()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            var roles = user?.Claims
                .Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
                .Select(c => c.Value)
                .ToList();

            var allSubCategories = await _unitOfWork.SubCategoryRepository.GetAllAsync();

            if (roles != null && (roles.Contains("Admin")))
            {
                return allSubCategories;
            }

            return allSubCategories.Where(sc => sc.IsActive);
        }

        public async Task<SubCategory> GetSubCategoryById(int id)
        {
            
            return await _unitOfWork.SubCategoryRepository.GetByIdAsync(id);
        }

        public async Task UpdateSubCategory(SubCategory subCategory)
        {
            var result = await _unitOfWork.SubCategoryRepository.GetByIdAsync(subCategory.Id);
            if (result != null)
            {
                await _unitOfWork.SubCategoryRepository.UpdateAsync(subCategory);
                await _unitOfWork.SaveChangeAsync();
            }                
        }
    }
}

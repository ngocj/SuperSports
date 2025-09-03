using Microsoft.AspNetCore.Http;
using SP.Application.Service.Interface;
using SP.Domain.Entity;
using SP.Infrastructure.UnitOfWork;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SP.Application.Service.Implement
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CategoryService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            var roles = user?.Claims
                .Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
                .Select(c => c.Value)
                .ToList();

            var allCategories = await _unitOfWork.CategoryRepository.GetAllAsync();

            if (roles != null && (roles.Contains("Admin")))
            {
                return allCategories; 
            }

            
            return allCategories.Where(c => c.IsActive);
        }

        public async Task<Category> GetCategoryById(int id)
        {
            return await _unitOfWork.CategoryRepository.GetByIdAsync(id);
        }

        public async Task CreateCategory(Category category)
        {
            await _unitOfWork.CategoryRepository.AddAsync(category);
            await _unitOfWork.SaveChangeAsync();
        }

        public async Task UpdateCategory(Category category)
        {
            var result = await _unitOfWork.CategoryRepository.GetByIdAsync(category.Id);
            if (result != null)
            {
                await _unitOfWork.CategoryRepository.UpdateAsync(category);
                await _unitOfWork.SaveChangeAsync();
            }
        }

        public async Task DeleteCategory(int id)
        {
            var result = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (result != null)
            {
                await _unitOfWork.CategoryRepository.DeleteAsync(result);
                await _unitOfWork.SaveChangeAsync();
            }
        }
    }
}

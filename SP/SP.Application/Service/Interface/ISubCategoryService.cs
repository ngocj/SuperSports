using SP.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Application.Service.Interface
{
    public interface ISubCategoryService
    {      
        Task<IEnumerable<SubCategory>> GetAllSubCategories();
        Task<SubCategory> GetSubCategoryById(int id);
        Task CreateSubCategory(SubCategory subCategory);
        Task UpdateSubCategory(SubCategory subCategory);
        Task DeleteSubCategory(int id);
    }
}

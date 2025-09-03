using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Application.Dto.CategoryDto
{
    public class CategoryViewDto
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public List<SubCategoryViewDto> SubCategories { get; set; } = new List<SubCategoryViewDto>();

    }
}

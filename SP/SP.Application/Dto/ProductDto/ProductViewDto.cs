using SP.Application.Dto.DiscountDto;
using SP.Application.Dto.ProductVariantDto;
using SP.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Application.Dto.ProductDto
{
    public class ProductViewDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int? DiscountId { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public List<VariantViewDto> ProductVariants { get; set; } = new List<VariantViewDto>();
        public string  SubCategoryName { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public int Percent { get; set; }
        public bool? IsDiscountActive { get; set; }
    }
}

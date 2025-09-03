using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Application.Dto.ProductDto
{
    public class ProductUpdateDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public int BrandId { get; set; }
        //public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public int? DiscountId { get; set; }
        public double? Rating { get; set; }
        public bool IsActive { get; set; }
    }
}

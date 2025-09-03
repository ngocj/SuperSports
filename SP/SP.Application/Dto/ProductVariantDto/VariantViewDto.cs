using SP.Application.Dto.ImageDto;
using SP.Application.Dto.OrderDetailDto;
using SP.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Application.Dto.ProductVariantDto
{
    public class VariantViewDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public double? Rating { get; set; }
        public int Quantity { get; set; }
        public bool IsActive { get; set; }
        public List<ImageFileDto> Images { get; set; } = new List<ImageFileDto>();


    }
}

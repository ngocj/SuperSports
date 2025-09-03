using Microsoft.AspNetCore.Http;
using SP.Application.Dto.ImageDto;
using SP.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Application.Dto.ProductVariantDto
{
    public class VariantCreateDto
    {
        public int ProductId { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public bool IsActive { get; set; }
        public List<IFormFile> ?Images { get; set; } // ảnh up lên
    }
}

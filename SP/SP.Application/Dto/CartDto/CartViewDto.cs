using SP.Application.Dto.ProductVariantDto;
using SP.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Application.Dto.CartDto
{
    public class CartViewDto
    {
        public Guid UserId { get; set; }
        public int ProductVariantId { get; set; }
        public int Quantity { get; set; }
        public VariantViewDto ProductVariant { get; set; }
    }
}

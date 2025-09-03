using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Application.Dto.CartDto
{
    public class CartCreateDto
    {
        public Guid UserId { get; set; }
        public int ProductVariantId { get; set; }
        public int Quantity { get; set; }
    }
}

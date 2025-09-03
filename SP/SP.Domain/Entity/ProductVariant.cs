using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Domain.Entity
{
    public class ProductVariant : Base
    {
        public int ProductId { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public double? Rating { get; set; }
        public int Quantity { get; set; }
        public bool IsActive { get; set; }
        public Product Product { get; set; }
        public List<Cart> Carts { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public List<Image>? Images { get; set; }


    }
}

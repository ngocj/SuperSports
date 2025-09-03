using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Domain.Entity
{
    public class OrderDetail
    {
        public Guid OrderId { get; set; } 
        public int ProductVariantId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Order Order { get; set; }
        public ProductVariant ProductVariant { get; set; }
        public List<FeedBack> FeedBacks { get; set; }

       
    }
}

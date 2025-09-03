using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Domain.Entity
{
    public class FeedBack : Base
    {
        public Guid OrderId { get; set; }
        public int ProductVariantId { get; set; }
        public Guid UserId { get; set; }
        public string? Comment { get; set; }
        public double Rating { get; set; }
        public User User { get; set; }
        public OrderDetail OrderDetail { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Domain.Entity
{
    public class Brand :Base
    {
        public string BrandName { get; set; }
        // description
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public List<Product> Products { get; set; }

    }
}

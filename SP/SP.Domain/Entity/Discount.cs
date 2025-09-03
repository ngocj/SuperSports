using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Domain.Entity
{
    public class Discount : Base
    {  
        public string Name { get; set; }
        public string? Description { get; set; }
        public int Percent { get; set; }       
        public bool IsActive { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public List<Product> Products { get; set; }
   
    }
}

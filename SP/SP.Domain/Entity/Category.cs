using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Domain.Entity
{
    public class Category : Base
    {
        public string CategoryName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public List<SubCategory> SubCategories { get; set; }
    }
}

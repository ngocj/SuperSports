using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Domain.Entity
{
    public class Province: Base
    {
        public string Name { get; set; }
        public List<District> Districts { get; set; }
    }
}

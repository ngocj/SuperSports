using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Domain.Entity
{
    public class District : Base
    {
        public string Name { get; set; }
        public int ProvinceId { get; set; }
        public Province Province { get; set; }
        public List<Ward> Wards { get; set; }
    }
}

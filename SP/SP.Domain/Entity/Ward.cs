using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Domain.Entity
{
    public class Ward : Base
    {
        public string WardName { get; set; }
        public int DistrictId  { get; set; }
        public District District { get; set; }
        public List<User> Users { get; set; }
        public List<Employee> Employees { get; set; }

    }
}

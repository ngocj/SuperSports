using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Domain.Entity
{
    public class Role : Base
    {      
        public string RoleName { get; set; }
        public List<User> Users { get; set; }
        public List<Employee> Employees { get; set; }
    }
}

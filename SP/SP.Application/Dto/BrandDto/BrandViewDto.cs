using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Application.Dto.BrandDto
{
    public class BrandViewDto
    {
        public int Id { get; set; }
        public string BrandName { get; set; }  
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}

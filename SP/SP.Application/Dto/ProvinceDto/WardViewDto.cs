using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Application.Dto.ProvinceDto
{
    public class WardViewDto
    {
        public int DistrictId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public DistrictViewDto District { get; set; }

    }
}

using SP.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Application.Dto.DiscountDto
{
    public class DiscountViewDto
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public int Percent { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
    }
}

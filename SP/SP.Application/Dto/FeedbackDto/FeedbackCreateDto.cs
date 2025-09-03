using SP.Application.Dto.OrderDetailDto;
using SP.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Application.Dto.FeedbackDto
{
    public class FeedbackCreateDto
    {
        public Guid OrderId { get; set; }
        public int ProductVariantId { get; set; }
        public Guid UserId { get; set; }
        public string? Comment { get; set; }
        public int Rating { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public OrderDetailViewDto? OrderDetail { get; set; }

    }
}

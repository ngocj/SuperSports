using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Application.Dto.FeedbackDto
{
    public class FeedbackUpdateDto
    {
        public Guid OrderId { get; set; }
        public int ProductVariantId { get; set; }
        public Guid UserId { get; set; }
        public string? Comment { get; set; }
        public double Rating { get; set; }
    }
}

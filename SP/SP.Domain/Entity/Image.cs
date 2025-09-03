using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Domain.Entity
{
    public class Image : Base
    {
        public string FileName { get; set; }
        public byte[] FileData { get; set; }
        public string ContentType { get; set; }
        public int ProductVariantId { get; set; }
        public ProductVariant ProductVariant { get; set; }  

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Application.Dto.ImageDto
{
    public class ImageFileDto
    {
        public int Id { get; set; }
        public int ProductVariantId { get; set; }
        public string FileName { get; set; }
        public byte[] FileData { get; set; }
        public string ContentType { get; set; }

        public Stream OpenReadStream()
        {
            throw new NotImplementedException();
        }
    }
}

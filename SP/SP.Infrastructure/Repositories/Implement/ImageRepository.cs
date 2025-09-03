using Microsoft.EntityFrameworkCore;
using SP.Domain.Entity;
using SP.Infrastructure.Context;
using SP.Infrastructure.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Infrastructure.Repositories.Implement
{
    public class ImageRepository : GenericRepository<Image>, IImageRepository
    {
        public ImageRepository(SPContext context) : base(context)
        {
        }

        public async Task<List<Image>> GetAllFileAsync()
        {
            return await _SPContext.Images.ToListAsync();
        }
    }
   
}

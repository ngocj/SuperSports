using SP.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Infrastructure.Repositories.Interface
{
    public interface IImageRepository : IGenericRepository<Image> 
    {
        Task<List<Image>> GetAllFileAsync();
        
    }
}

using SP.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Application.Service.Interface
{
    public interface IBrandService
    {
        Task<IEnumerable<Brand>> GetAllBrands();
        Task<Brand> GetBrandById(int id);
        Task CreateBrand(Brand brand);
        Task UpdateBrand(Brand brand);
        Task DeleteBrand(int id);
    }
}

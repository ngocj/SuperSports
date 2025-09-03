using SP.Domain.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SP.Infrastructure.Repositories.Interface
{
    public interface IDistrictRepository : IGenericRepository<District>
    {
        Task<IEnumerable<District>> GetDistrictsByProvinceIdAsync(int provinceId);
    }
}

using Microsoft.EntityFrameworkCore;
using SP.Domain.Entity;
using SP.Infrastructure.Context;
using SP.Infrastructure.Repositories.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SP.Infrastructure.Repositories.Implement
{
    public class DistrictRepository : GenericRepository<District>, IDistrictRepository
    {
        public DistrictRepository(SPContext context) : base(context)
        {
        }

        public async Task<IEnumerable<District>> GetDistrictsByProvinceIdAsync(int provinceId)
        {
            return await _SPContext.Districts
                .Where(d => d.ProvinceId == provinceId)
                .ToListAsync();
        }
    }
}

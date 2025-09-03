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
    public class WardRepository : GenericRepository<Ward>, IWardRepository
    {
        public WardRepository(SPContext sPContext) : base(sPContext)
        {

        }
        public override async Task<Ward> GetByIdAsync(int id)
        {
            return await _SPContext.Set<Ward>()
                .Include(x => x.District)
                .ThenInclude(x => x.Province)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Ward>> GetWardsByDistrictIdAsync(int districtId)
        {
            return await _SPContext.Wards
                .Where(d => d.DistrictId == districtId)
                .ToListAsync();

        }
    }
}

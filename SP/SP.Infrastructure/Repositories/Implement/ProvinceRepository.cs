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
    public class ProvinceRepository : GenericRepository<Province>, IProvinceRepository
    {
        public ProvinceRepository(SPContext sPContext) : base(sPContext)
        {
        }
        public override async Task<Province> GetByIdAsync(int id)
        {
            return await _SPContext.Provinces
                .Include(dt => dt.Districts)
                .ThenInclude(w => w.Wards)
                .FirstOrDefaultAsync(pv => pv.Id == id);
        }

    }
}

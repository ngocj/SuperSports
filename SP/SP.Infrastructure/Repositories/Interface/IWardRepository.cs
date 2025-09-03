using SP.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Infrastructure.Repositories.Interface
{
    public interface IWardRepository : IGenericRepository<Ward>
    {
        Task<IEnumerable<Ward>> GetWardsByDistrictIdAsync(int districtId);    

    }
}

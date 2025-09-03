using SP.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Application.Service.Interface
{
    public interface IWardService
    {
        Task<IEnumerable<Ward>> GetAllWards();
        Task<Ward> GetAllWardById(int id);
        Task<IEnumerable<Ward>> GetWardsByDistrictIdAsync(int districtId);
    }
}

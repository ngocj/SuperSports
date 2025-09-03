using SP.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Application.Service.Interface
{
    public interface IDistrictService
    {
        Task<IEnumerable<District>> GetAllDistricts();
        Task<District> GetAllDistrictById(int id);
        Task<IEnumerable<District>> GetDistrictsByProvinceIdAsync(int provinceId);
    }
}

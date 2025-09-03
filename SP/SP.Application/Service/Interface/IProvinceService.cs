using SP.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Application.Service.Interface
{
    public interface IProvinceService
    {
        Task<IEnumerable<Province>> GetAllProvinces();
        Task<Province> GetAllProvinceById(int id);
    }
}

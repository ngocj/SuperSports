using SP.Application.Service.Interface;
using SP.Domain.Entity;
using SP.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Application.Service.Implement
{
    public class ProvinceService : IProvinceService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProvinceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Province> GetAllProvinceById(int id)
        {          
            return await _unitOfWork.ProvinceRepository.GetByIdAsync(id);

        }
        public async Task<IEnumerable<Province>> GetAllProvinces()
        {
            return await _unitOfWork.ProvinceRepository.GetAllAsync();
        }
    }
}

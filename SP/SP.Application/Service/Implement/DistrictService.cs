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
    public class DistrictService : IDistrictService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DistrictService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<District> GetAllDistrictById(int id)
        {
            return await _unitOfWork.DistrictRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<District>> GetAllDistricts()
        {
           return await _unitOfWork.DistrictRepository.GetAllAsync();
        }

        public async Task<IEnumerable<District>> GetDistrictsByProvinceIdAsync(int provinceId)
        {
            return await _unitOfWork.DistrictRepository.GetDistrictsByProvinceIdAsync(provinceId);
        }
    }
}

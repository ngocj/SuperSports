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
    public class WardService : IWardService
    {
        private readonly IUnitOfWork _unitOfWork;
        public WardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Ward> GetAllWardById(int id)
        {
           return await _unitOfWork.WardRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Ward>> GetAllWards()
        {
          return await _unitOfWork.WardRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Ward>> GetWardsByDistrictIdAsync(int districtId)
        {
            return await _unitOfWork.WardRepository.GetWardsByDistrictIdAsync(districtId);
        }
    }
}

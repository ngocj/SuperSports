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
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateRole(Role role)
        {
            await _unitOfWork.RoleRepository.AddAsync(role);
            await _unitOfWork.SaveChangeAsync();
        }

        public async Task DeleteRole(int id)
        {
            var result = await _unitOfWork.RoleRepository.GetByIdAsync(id);
            if (result != null)
            {
                await _unitOfWork.RoleRepository.DeleteAsync(result);
                await _unitOfWork.SaveChangeAsync();
            }
        }
        public async Task<IEnumerable<Role>> GetAllRoles()
        {
            return await _unitOfWork.RoleRepository.GetAllAsync();
        }

        public async Task<Role> GetRoleById(int id)
        {
            return await _unitOfWork.RoleRepository.GetByIdAsync(id);
        }

        public async Task UpdateRole(Role role)
        {
            var result = await _unitOfWork.RoleRepository.GetByIdAsync(role.Id);
            if (result != null)
            {
                await _unitOfWork.RoleRepository.UpdateAsync(role);
                await _unitOfWork.SaveChangeAsync();
            }
        }
    }
}

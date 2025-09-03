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
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task CreateUser(User user)
        {       
            await _unitOfWork.UserRepository.AddAsync(user);
            await _unitOfWork.SaveChangeAsync();
            
        }

        public async Task DeleteUser(Guid id)
        {
            var result = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (result != null)
            {
                await _unitOfWork.UserRepository.DeleteAsync(result);
                await _unitOfWork.SaveChangeAsync();
            }
            
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {

            return await _unitOfWork.UserRepository.GetAllAsync();
            
        }

        public async Task<User> GetUserById(Guid id)
        {
            return await _unitOfWork.UserRepository.GetByIdAsync(id);
            
        }

        public  async Task UpdateUser(User user)
        {
            var result = await _unitOfWork.UserRepository.GetByIdAsync(user.Id);
            if (result != null)
            {
                await _unitOfWork.UserRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangeAsync();
            }
            
        }
    }
}

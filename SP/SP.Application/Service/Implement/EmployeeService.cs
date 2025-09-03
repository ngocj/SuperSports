using SP.Application.Service.Interface;
using SP.Domain.Entity;
using SP.Infrastructure.Repositories.Implement;
using SP.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.Application.Service.Implement
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        public EmployeeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public Task CreateEmployee(Employee employee)
        {
            _unitOfWork.EmployeeRepository.AddAsync(employee);
            return _unitOfWork.SaveChangeAsync();

            
        }

        public async Task DeleteEmployee(Guid id)
        {
            var result = await _unitOfWork.EmployeeRepository.GetByIdAsync(id);
            if (result != null)
            {
                await _unitOfWork.EmployeeRepository.DeleteAsync(result);
                await _unitOfWork.SaveChangeAsync();
            }
                              
        }

        public async Task<IEnumerable<Employee>> GetAllEmployees()
        {
            return await _unitOfWork.EmployeeRepository.GetAllAsync();
            
        }

        public async Task<IEnumerable<EmployeeRepository.EmployeeStatsDto>> GetAllEmployeeStatisticsAsync()
        {
           return await _unitOfWork.EmployeeRepository.GetAllEmployeeStatisticsAsync();
        }

        public async Task<IEnumerable<string>> GetCustomerNamesHandledByAsync(Guid employeeId)
        {
            return await _unitOfWork.EmployeeRepository.GetCustomerNamesHandledByAsync(employeeId);
        }

        public async Task<Employee> GetEmployeeById(Guid id)
        {
            return await _unitOfWork.EmployeeRepository.GetByIdAsync(id);
            
        }

        public async Task<int> GetHandledOrderCountAsync(Guid employeeId)
        {
            return await _unitOfWork.EmployeeRepository.GetHandledOrderCountAsync(employeeId);
        }

        public async Task<IEnumerable<EmployeeRepository.HandledOrderDto>> GetHandledOrdersByEmployeeAsync(Guid employeeId)
        {
            return await _unitOfWork.EmployeeRepository.GetHandledOrdersByEmployeeAsync(employeeId);
        }

        public async Task<decimal> GetRevenueByEmployeeAsync(Guid employeeId)
        {
            return await _unitOfWork.EmployeeRepository.GetRevenueByEmployeeAsync(employeeId);
        }

        public async Task UpdateEmployee(Employee employee)
        {
            var result = await _unitOfWork.EmployeeRepository.GetByIdAsync(employee.Id);
            if (result != null)
            {
                await _unitOfWork.EmployeeRepository.UpdateAsync(employee);
                await _unitOfWork.SaveChangeAsync();
            }
            
        }
    }
}

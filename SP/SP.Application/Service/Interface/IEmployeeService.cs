using SP.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SP.Infrastructure.Repositories.Implement.EmployeeRepository;

namespace SP.Application.Service.Interface
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetAllEmployees();
        Task<Employee> GetEmployeeById(Guid id);
        Task CreateEmployee(Employee employee);
        Task UpdateEmployee(Employee employee);
        Task DeleteEmployee(Guid id);

        // Tổng số đơn hàng mà nhân viên đã xử lý thành công
        Task<int> GetHandledOrderCountAsync(Guid employeeId);

        // Tổng doanh thu từ các đơn hàng mà nhân viên xử lý
        Task<decimal> GetRevenueByEmployeeAsync(Guid employeeId);

        // Danh sách các khách hàng mà nhân viên đã xử lý đơn
        Task<IEnumerable<string>> GetCustomerNamesHandledByAsync(Guid employeeId);
        Task<IEnumerable<HandledOrderDto>> GetHandledOrdersByEmployeeAsync(Guid employeeId);
        Task<IEnumerable<EmployeeStatsDto>> GetAllEmployeeStatisticsAsync();
    }

}

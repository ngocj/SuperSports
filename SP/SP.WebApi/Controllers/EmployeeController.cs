using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SP.Application.Dto.EmployeeDto;
using SP.Application.Service.Implement;
using SP.Application.Service.Interface;
using SP.Domain.Entity;
using SP.Infrastructure.Context;

namespace SP.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeService _employeeService;
        private readonly SPContext _context;

        public EmployeeController(IMapper mapper, IEmployeeService employeeService, SPContext context)
        {
            _mapper = mapper;
            _employeeService = employeeService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _employeeService.GetAllEmployees();
            var employeeDto = _mapper.Map<IEnumerable<EmployeeViewDto>>(employees);
            return Ok(employeeDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(Guid id)
        {
            var employee = await _employeeService.GetEmployeeById(id);
            if (employee == null)
            {
                return NotFound();
            }
            var employeeDto = _mapper.Map<EmployeeViewDto>(employee);
            return Ok(employeeDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeCreateDto employeeCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Kiểm tra email trùng
                if (await _context.Employees.AnyAsync(u => u.Email == employeeCreateDto.Email))
                {
                    return Conflict(new { field = "Email", message = "Email đã tồn tại." });
                }

                // Kiểm tra số điện thoại trùng
                if (await _context.Employees.AnyAsync(u => u.PhoneNumber == employeeCreateDto.PhoneNumber))
                {
                    return Conflict(new { field = "PhoneNumber", message = "Số điện thoại đã tồn tại." });
                }

                var employee = _mapper.Map<Employee>(employeeCreateDto);

                // Băm mật khẩu trước khi lưu
                employee.Password = BCrypt.Net.BCrypt.HashPassword(employeeCreateDto.Password);

                await _employeeService.CreateEmployee(employee);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi tạo người dùng.", detail = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateEmployee([FromBody] EmployeeUpdateDto employeeUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var employee = await _employeeService.GetEmployeeById(employeeUpdateDto.Id);
            if (employee == null)
            {
                return NotFound();
            }

            // ✅ Ánh xạ chỉ những field được phép update từ DTO vào entity đã có
            _mapper.Map(employeeUpdateDto, employee);
            employee.UpdatedAt = DateTime.Now;

            await _employeeService.UpdateEmployee(employee);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(Guid id)
        {
            var employee = await _employeeService.GetEmployeeById(id);
            if (employee == null)
            {
                return NotFound();
            }
            await _employeeService.DeleteEmployee(id);
            return NoContent();
        }

        [HttpGet("handled-order-count/{employeeId}")]
        public async Task<IActionResult> GetHandledOrderCount(Guid employeeId)
        {
            var count = await _employeeService.GetHandledOrderCountAsync(employeeId);
            return Ok(count);
        }
        [HttpGet("revenue/{employeeId}")]
        public async Task<IActionResult> GetRevenueByEmployee(Guid employeeId)
        {
            var revenue = await _employeeService.GetRevenueByEmployeeAsync(employeeId);
            return Ok(revenue);
        }
        [HttpGet("customers-handled/{employeeId}")]
        public async Task<IActionResult> GetCustomerNamesHandledBy(Guid employeeId)
        {
            var customerNames = await _employeeService.GetCustomerNamesHandledByAsync(employeeId);
            return Ok(customerNames);
        }

        [HttpGet("stats/{employeeId}")]
        public async Task<IActionResult> GetEmployeeStats(Guid employeeId)
        {
            try
            {
                var stats = new
                {
                    HandledOrderCount = await _employeeService.GetHandledOrderCountAsync(employeeId),
                    Revenue = await _employeeService.GetRevenueByEmployeeAsync(employeeId),
                    CustomersHandled = await _employeeService.GetCustomerNamesHandledByAsync(employeeId),
                    HandledOrders = await _employeeService.GetHandledOrdersByEmployeeAsync(employeeId)
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi lấy thống kê.", detail = ex.Message });
            }
        }

        [HttpGet("handled-orders/{employeeId}")]
        public async Task<IActionResult> GetHandledOrders(Guid employeeId)
        {
            try
            {
                var orders = await _employeeService.GetHandledOrdersByEmployeeAsync(employeeId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi lấy danh sách đơn hàng.", detail = ex.Message });
            }
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetAllEmployeeStatistics()
        {
            try
            {
                var stats = await _employeeService.GetAllEmployeeStatisticsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi lấy thống kê nhân viên.", detail = ex.Message });
            }
        }

    }
}

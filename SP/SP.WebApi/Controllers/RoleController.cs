using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SP.Application.Dto.RoleDto;
using SP.Application.Service.Interface;
using SP.Domain.Entity;
using SP.Infrastructure.UnitOfWork;
using System.Security.AccessControl;

namespace SP.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {   
        private readonly IMapper _mapper;
        private readonly IRoleService _roleService;

        public RoleController(IMapper mapper, IRoleService roleService)
        {
            _mapper = mapper;
            _roleService = roleService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var role = await _roleService.GetAllRoles();
            var roleDto = _mapper.Map<IEnumerable<RoleViewDto>>(role);
            return Ok(roleDto);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            var role = await _roleService.GetRoleById(id);
            if (role == null)
            {
                return NotFound();
            }
            var roleDto = _mapper.Map<RoleViewDto>(role);
            return Ok(roleDto);
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] RoleCreateDto roleCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var role = _mapper.Map<Role>(roleCreateDto);
            await _roleService.CreateRole(role);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> UpdateRole([FromBody] RoleViewDto roleUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var role = await _roleService.GetRoleById(roleUpdateDto.Id);
            if (role == null)
            {
                return NotFound();
            }
            var roleDto = _mapper.Map<Role>(roleUpdateDto);
            await _roleService.UpdateRole(roleDto);
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _roleService.GetRoleById(id);
            if (role == null)
            {
                return NotFound();
            }
            await _roleService.DeleteRole(id);
            return Ok();
        }
    }
}

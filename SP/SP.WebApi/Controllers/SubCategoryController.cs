using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SP.Application.Dto.CategoryDto;
using SP.Application.Service.Interface;
using SP.Domain.Entity;

namespace SP.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubCategoryController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISubCategoryService _subCategoryService;

        public SubCategoryController(IMapper mapper, ISubCategoryService subCategoryService)
        {
            _mapper = mapper;
            _subCategoryService = subCategoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSubCategories()
        {
            var subCategories = await _subCategoryService.GetAllSubCategories();
            var subCategoryDto = _mapper.Map<IEnumerable<SubCategoryViewDto>>(subCategories);
            return Ok(subCategoryDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubCategoryById(int id)
        {
            var subCategory = await _subCategoryService.GetSubCategoryById(id);
            if (subCategory == null)
            {
                return NotFound();
            }
            var subCategoryDto = _mapper.Map<SubCategoryViewDto>(subCategory);
            return Ok(subCategoryDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubCategory([FromBody] SubCategoryCreateDto subCategoryCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var subCategory = _mapper.Map<SubCategory>(subCategoryCreateDto);
                await _subCategoryService.CreateSubCategory(subCategory);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateSubCategory([FromBody] SubCategoryViewDto subCategoryUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var subCategory = _mapper.Map<SubCategory>(subCategoryUpdateDto);
                await _subCategoryService.UpdateSubCategory(subCategory);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubCategory(int id)
        {
            var subCategory = await _subCategoryService.GetSubCategoryById(id);
            if (subCategory == null)
            {
                return NotFound();
            }

            await _subCategoryService.DeleteSubCategory(id);
            return Ok();
        }

    }
}

using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SP.Application.Dto.BrandDto;
using SP.Application.Dto.CategoryDto;
using SP.Application.Service.Interface;
using SP.Domain.Entity;

namespace SP.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IBrandService _brandService;

        public BrandController(IMapper mapper, IBrandService brandService)
        {
            _mapper = mapper;
            _brandService = brandService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBrands()
        {
            var brands = await _brandService.GetAllBrands();
            var brandDto = _mapper.Map<IEnumerable<BrandViewDto>>(brands);
            return Ok(brandDto);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrandById(int id)
        {
            var brand = await _brandService.GetBrandById(id);
            if (brand == null)
            {
                return NotFound();
            }
            var brandDto = _mapper.Map<BrandViewDto>(brand);
            return Ok(brandDto);
        }
        [HttpPost]
        public async Task<IActionResult> CreateBrand([FromBody] BrandCreateDto brandCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var brand = _mapper.Map<Brand>(brandCreateDto);
            await _brandService.CreateBrand(brand);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> UpdateCategory([FromBody] BrandViewDto brandViewDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var category = await _brandService.GetBrandById(brandViewDto.Id);
            if (category == null)
            {
                return NotFound();
            }
            var brandViewDto1 = _mapper.Map<Brand>(brandViewDto);
            await _brandService.UpdateBrand(brandViewDto1);
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            var brand = await _brandService.GetBrandById(id);
            if (brand == null)
            {
                return NotFound();
            }
            await _brandService.DeleteBrand(id);
            return Ok();
        }
    }
}

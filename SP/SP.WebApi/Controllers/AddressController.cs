using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SP.Application.Dto.ProvinceDto;
using SP.Application.Service.Interface;

namespace SP.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProvinceService _provinceService;
        private readonly IDistrictService _districtService;
        private readonly IWardService _wardService;

        public AddressController(
            IMapper mapper,
            IProvinceService provinceService,
            IDistrictService districtService,
            IWardService wardService)
        {
            _mapper = mapper;
            _provinceService = provinceService;
            _districtService = districtService;
            _wardService = wardService;
        }

        [HttpGet("provinces")]
        public async Task<IActionResult> GetAllProvinces()
        {
            var provinces = await _provinceService.GetAllProvinces();
            var provinceDto = _mapper.Map<IEnumerable<ProvinceViewDto>>(provinces);
            return Ok(provinceDto);
        }

        [HttpGet("provinces/{id}")]
        public async Task<IActionResult> GetProvinceById(int id)
        {
            var province = await _provinceService.GetAllProvinceById(id);
            if (province == null) return NotFound();
            var provinceDto = _mapper.Map<ProvinceViewDto>(province);
            return Ok(provinceDto);
        }
        [HttpGet("ward")]
        public async Task<IActionResult> GetAllWard()
        {
            var provinces = await _wardService.GetAllWards();
            var provinceDto = _mapper.Map<IEnumerable<WardViewDto>>(provinces);
            return Ok(provinceDto);
        }

        [HttpGet("ward/{id}")]
        public async Task<IActionResult> GetWardById(int id)
        {
            var province = await _wardService.GetAllWardById(id);
            if (province == null) return NotFound();
            var provinceDto = _mapper.Map<WardViewDto>(province);
            return Ok(provinceDto);
        }



        [HttpGet("districts/{provinceId}")]
        public async Task<IActionResult> GetDistrictsByProvinceId(int provinceId)
        {
            var districts = await _districtService.GetDistrictsByProvinceIdAsync(provinceId);
            var districtDto = _mapper.Map<IEnumerable<DistrictViewDto>>(districts);
            return Ok(districtDto);
        }

        [HttpGet("district/{id}")]
        public async Task<IActionResult> GetDistrictById(int id)
        {
            var district = await _districtService.GetAllDistrictById(id);
            if (district == null) return NotFound();
            var districtDto = _mapper.Map<DistrictViewDto>(district);
            return Ok(districtDto);
        }

        [HttpGet("wards/{districtId}")]
        public async Task<IActionResult> GetWardsByDistrictId(int districtId)
        {
            var wards = await _wardService.GetWardsByDistrictIdAsync(districtId);
            var wardDto = _mapper.Map<IEnumerable<WardViewDto>>(wards);
            return Ok(wardDto);
        }
    }
}

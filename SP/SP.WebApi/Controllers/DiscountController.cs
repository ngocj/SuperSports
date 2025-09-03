using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SP.Application.Dto.DiscountDto;
using SP.Application.Service.Interface;
using SP.Domain.Entity;

namespace SP.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDiscountService _discountService; 
        public DiscountController(IMapper mapper, IDiscountService discountService)
        {
            _mapper = mapper;
            _discountService = discountService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllDiscount()
        {
            var discounts = await _discountService.GetAllDiscounts();
            var discountDto = _mapper.Map<IEnumerable<DiscountViewDto>>(discounts);
            return Ok(discountDto);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDiscountById(int id)
        {
            var discount = await _discountService.GetDiscountById(id);
            if (discount == null)
            {
                return NotFound();
            }
            var discountDto = _mapper.Map<DiscountViewDto>(discount);
            return Ok(discountDto);
        }   
        [HttpPost]
        public async Task<IActionResult> CreateDiscount([FromBody] DiscountCreateDto discountCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var discount = _mapper.Map<Discount>(discountCreateDto);
            await _discountService.CreateDiscount(discount);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> UpdateDiscount([FromBody] DiscountViewDto discountViewDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var discount = await _discountService.GetDiscountById(discountViewDto.Id);
            if (discount == null)
            {
                return NotFound();
            }
            var discountToUpdate = _mapper.Map<Discount>(discountViewDto);
            await _discountService.UpdateDiscount(discountToUpdate);
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiscount(int id)
        {
            var discount = await _discountService.GetDiscountById(id);
            if (discount == null)
            {
                return NotFound();
            }
            await _discountService.DeleteDiscount(id);
            return Ok();

        }    
    }
}

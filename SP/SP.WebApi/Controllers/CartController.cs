using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SP.Application.Dto.CartDto;
using SP.Application.Service.Interface;
using SP.Domain.Entity;

namespace SP.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICartService _cartService;

        public CartController(IMapper mapper, ICartService cartService)
        {
            _mapper = mapper;
            _cartService = cartService;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllCarts()
        {
            var carts = await _cartService.GetAllCarts();
            var cartDto = _mapper.Map<IEnumerable<CartViewDto>>(carts);
            return Ok(cartDto);
        }
        [HttpGet("{userId}/{productVariantId}")]
        public async Task<IActionResult> GetCartById(Guid userId, int productVariantId)
        {
            var cart = await _cartService.GetCartById(userId, productVariantId);
            if (cart == null)
            {
                return NotFound();
            }
            var cartDto = _mapper.Map<CartViewDto>(cart);
            return Ok(cartDto);
        }


        [HttpPost]
        public async Task<IActionResult> CreateCart([FromBody] CartCreateDto cartCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var cart = _mapper.Map<Cart>(cartCreateDto);
            await _cartService.CreateCart(cart);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCart([FromBody] CartUpdateDto cartUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var cart = await _cartService.GetCartById(cartUpdateDto.UserId, cartUpdateDto.ProductVariantId);
            if (cart == null)
            {
                return NotFound();
            }
            var updatedCart = _mapper.Map<Cart>(cartUpdateDto);
            await _cartService.UpdateCart(updatedCart);
            return Ok();
        }
        [HttpDelete("{userId}/{productVariantId}")]
        public async Task<IActionResult> DeleteCart(Guid userId, int productVariantId)
        {
            var cart = await _cartService.GetCartById(userId, productVariantId);
            if (cart == null)
            {
                return NotFound();
            }
            await _cartService.DeleteCart(userId, productVariantId);
            return Ok();
        }
        [Authorize]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAllCartsByUserId(Guid userId)
        {
            var carts = await _cartService.GetAllCartsByUserIdAsync(userId);
            var cartDto = _mapper.Map<IEnumerable<CartViewDto>>(carts);
            return Ok(cartDto);
        }
        
    }
}

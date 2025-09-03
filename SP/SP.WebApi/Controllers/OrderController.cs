using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SP.Application.Dto.OrderDto;
using SP.Application.Service.Interface;
using SP.Domain.Entity;
using SP.Infrastructure.Context;

namespace SP.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IOrderService _orderService;
        private readonly SPContext _sPContext;

        public OrderController(IMapper mapper, IOrderService orderService, SPContext sPContext)
        {
            _mapper = mapper;
            _orderService = orderService;
            _sPContext = sPContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrders();
            var orderDto = _mapper.Map<IEnumerable<OrderViewDto>>(orders);
            return Ok(orderDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var order = await _orderService.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }
            var orderDto = _mapper.Map<OrderViewDto>(order);
            return Ok(orderDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto orderCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (orderCreateDto.Id == Guid.Empty)
            {
                orderCreateDto.Id = Guid.NewGuid();
            }

            var user = await _sPContext.Users.FirstOrDefaultAsync(u => u.Id == orderCreateDto.UserId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            user.WardId = orderCreateDto.WardId;
            _sPContext.Users.Update(user);

            // 1. Map sang Order entity (gồm cả OrderDetails nếu DTO có chứa)
            var order = _mapper.Map<Order>(orderCreateDto);

            // 2. Duyệt qua từng OrderDetail và xử lý tồn kho
            foreach (var detail in order.OrderDetails)
            {
                var productVariant = await _sPContext.ProductVariants
                    .FirstOrDefaultAsync(pv => pv.Id == detail.ProductVariantId);

                if (productVariant == null)
                {
                    return BadRequest($"Không tìm thấy biến thể sản phẩm với ID: {detail.ProductVariantId}");
                }

                if (productVariant.Quantity < detail.Quantity)
                {
                    return BadRequest($"Sản phẩm '{productVariant.Product.ProductName}' không đủ hàng trong kho.");
                }

                // Trừ số lượng tồn kho
                productVariant.Quantity -= detail.Quantity;

                // Cập nhật lại biến thể
                _sPContext.ProductVariants.Update(productVariant);
            }

            // 3. Thêm Order vào DbContext
            await _sPContext.Orders.AddAsync(order);

            // 4. Lưu toàn bộ thay đổi
            await _sPContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateOrder([FromBody] OrderUpdateDto orderUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _orderService.GetOrderById(orderUpdateDto.Id);
            if (order == null)
            {
                return NotFound();
            }

            // Cập nhật các trường cho phép từ DTO sang entity hiện có
            _mapper.Map(orderUpdateDto, order);

            await _orderService.UpdateOrder(order);

            return Ok();
        }

        [HttpDelete("{id}")]    
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            var order = await _orderService.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }
            await _orderService.DeleteOrder(id);
            return Ok();
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserOrders(Guid userId)
        {
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
           
            var orderDtos = _mapper.Map<List<OrderViewDto>>(orders);

            return Ok(orderDtos);
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(Guid id)
        {
            var order = await _orderService.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }

            var result = await _orderService.CancelOrderAsync(id);
            if (!result)
            {
                return BadRequest("Không thể hủy đơn hàng này.");
            }

            return Ok("Đơn hàng đã được hủy thành công.");
        }

    }
}

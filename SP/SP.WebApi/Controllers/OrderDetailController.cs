using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SP.Application.Dto.OrderDetailDto;
using SP.Application.Service.Interface;
using SP.Domain.Entity;

namespace SP.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IOrderDetailService _orderDetailService;

        public OrderDetailController(IMapper mapper, IOrderDetailService orderDetailService)
        {
            _mapper = mapper;
            _orderDetailService = orderDetailService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrderDetails()
        {
            var orderDetails = await _orderDetailService.GetAllOrderDetails();
            var orderDetailDto = _mapper.Map<IEnumerable<OrderDetailViewDto>>(orderDetails);
            return Ok(orderDetailDto);
        }
        [HttpGet("{orderId}/{productVariantId}")]
        public async Task<IActionResult> GetOrderDetailById(Guid orderId, int productVariantId)
        {
            var orderDetail = await _orderDetailService.GetOrderDetailById(orderId, productVariantId);
            if (orderDetail == null)
            {
                return NotFound();
            }
            var orderDetailDto = _mapper.Map<OrderDetailViewDto>(orderDetail);
            return Ok(orderDetailDto);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrderDetail([FromBody] OrderDetailCreateDto orderDetailCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var orderDetail = _mapper.Map<OrderDetail>(orderDetailCreateDto);
            await _orderDetailService.CreateOrderDetail(orderDetail);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> UpdateOrderDetail([FromBody] OrderDetailViewDto orderDetailViewDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var orderDetail = _mapper.Map<OrderDetail>(orderDetailViewDto);
            await _orderDetailService.UpdateOrderDetail(orderDetail);
            return Ok();
        }
        [HttpDelete("{orderId}/{productVariantId}")]
        public async Task<IActionResult> DeleteOrderDetail(Guid orderId, int productVariantId)
        {
            var orderDetail = await _orderDetailService.GetOrderDetailById(orderId, productVariantId);
            if (orderDetail == null)
            {
                return NotFound();
            }
            await _orderDetailService.DeleteOrderDetail(orderId, productVariantId);
            return Ok();
        }
        [HttpGet("revenue/total")]
        public async Task<IActionResult> GetTotalRevenue()
        {
            var total = await _orderDetailService.GetTotalRevenueAsync();
            return Ok(total);
        }

        [HttpGet("revenue/total-by-range")]
        public async Task<IActionResult> GetTotalRevenueByDateRange([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var total = await _orderDetailService.GetTotalRevenueAsync(from, to);
            return Ok(total);
        }

        [HttpGet("orders/completed-count")]
        public async Task<IActionResult> GetCompletedOrderCount()
        {
            var count = await _orderDetailService.GetCompletedOrderCountAsync();
            return Ok(count);
        }

        [HttpGet("products/total-sold")]
        public async Task<IActionResult> GetTotalProductsSold()
        {
            var total = await _orderDetailService.GetTotalProductSoldAsync();
            return Ok(total);
        }

        [HttpGet("products/top-selling")]
        public async Task<IActionResult> GetTopSellingVariants([FromQuery] int top = 5)
        {
            var topVariants = await _orderDetailService.GetTopSellingVariantsAsync(top);
            return Ok(topVariants);
        }

        [HttpGet("products/total-pending")]
        public async Task<IActionResult> GetTotalProductsPending()
        {
            var total = await _orderDetailService.GetTotalProductPendingAsync();
            return Ok(total);
        }

        [HttpGet("products/total-delivered")]
        public async Task<IActionResult> GetTotalProductsDelivered()
        {
            var total = await _orderDetailService.GetTotalProductDeliveredAsync();
            return Ok(total);
        }

        [HttpGet("products/total-canceled")]
        public async Task<IActionResult> GetTotalProductsCanceled()
        {
            var total = await _orderDetailService.GetTotalProductCanceledAsync();
            return Ok(total);
        }

        [HttpGet("products/total-shipping")]
        public async Task<IActionResult> GetTotalProductsShipping()
        {
            var total = await _orderDetailService.GetTotalProductShippingAsync();
            return Ok(total);
        }

        [HttpGet("orders/total-count")]
        public async Task<IActionResult> GetTotalOrderCount()
        {
            var count = await _orderDetailService.GetTotalOrderCountAsync();
            return Ok(count);
        }   

        [HttpGet("orders/avg-order-value")]
        public async Task<IActionResult> GetAverageOrderValue()
        {
            var avgValue = await _orderDetailService.GetAverageOrderValueAsync();
            return Ok(avgValue);
        }

        [HttpGet("orders/canceled-count")]
        public async Task<IActionResult> GetCanceledOrderCount()
        {
            var count = await _orderDetailService.GetCanceledOrderCountAsync();
            return Ok(count);
        }

        [HttpGet("customers/top-spending")]
        public async Task<IActionResult> GetTopCustomers([FromQuery] int count = 5)
        {
            var customers = await _orderDetailService.GetTopCustomersAsync(count);
            return Ok(customers);
        }

        [HttpGet("revenue/by-period")]
        public async Task<IActionResult> GetRevenueByPeriod([FromQuery] string period = "month")
        {
            var revenue = await _orderDetailService.GetRevenueByPeriodAsync(period);
            return Ok(revenue);
        }
    }
}

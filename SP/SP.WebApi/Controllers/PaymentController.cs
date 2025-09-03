using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SP.Application.Service.Interface;
using SP.WebApi.VnPay;

namespace SP.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;

        public PaymentController(IVnPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }

        [HttpPost("create-payment-url")]
        public IActionResult CreatePaymentUrlVnpay([FromBody] PaymentInformationModel model)
        {
            try
            {
                var url = _vnPayService.CreatePaymentUrl(model, HttpContext);
                if (url == null)
                    return BadRequest("URL is null.");
                return Ok(url);
            }
            catch (Exception ex)
            {
                // Log ex.Message
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("payment-callback")]
        public IActionResult PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            return Ok(response);
        }
    
    }

}

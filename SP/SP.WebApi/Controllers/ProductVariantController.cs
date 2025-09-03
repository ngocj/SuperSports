using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SP.Application.Dto.ProductVariantDto;
using SP.Application.Service.Interface;
using SP.Domain.Entity;

namespace SP.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductVariantController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProductVariantService _productVariantService;
        private readonly IImageService _imageService;

        public ProductVariantController(IMapper mapper, IProductVariantService productVariantService, IImageService imageService)
        {
            _mapper = mapper;
            _productVariantService = productVariantService;
            _imageService = imageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProductVariants()
        {
            var productVariants = await _productVariantService.GetAllProductVariants();
            var productVariantDto = _mapper.Map<IEnumerable<VariantViewDto>>(productVariants);
            return Ok(productVariantDto);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductVariantById(int id)
        {
            var productVariant = await _productVariantService.GetProductVariantById(id);
            if (productVariant == null)
            {
                return NotFound();
            }
            var productVariantDto = _mapper.Map<VariantViewDto>(productVariant);
            return Ok(productVariantDto);
        }
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateProductVariant([FromForm] VariantCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 1. Map và lưu ProductVariant
            var productVariant = new ProductVariant
            {
                ProductId = dto.ProductId,
                Color = dto.Color,
                Size = dto.Size,
                Price = dto.Price,
                Quantity = dto.Quantity,
                IsActive = dto.IsActive
            };

            await _productVariantService.CreateProductVariant(productVariant);

            // 2. Lưu ảnh với ProductVariantId
            if (dto.Images != null && dto.Images.Count > 0)
            {
                foreach (var file in dto.Images)
                {
                    await _imageService.UploadFileAsync(file, productVariant.Id);
                }
            }

            return Ok(new { message = "Tạo sản phẩm và ảnh thành công" });
        }


        [HttpPut]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateProductVariant([FromForm] VariantUpdateDto variantUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productVariant = await _productVariantService.GetProductVariantById(variantUpdateDto.Id);
            if (productVariant == null)
            {
                return NotFound();
            }

            // 1. Update thông tin product variant (trừ Images)
            var updatedProductVariant = _mapper.Map<ProductVariant>(variantUpdateDto);
            await _productVariantService.UpdateProductVariant(updatedProductVariant);
      

            // 3. Upload ảnh mới
            if (variantUpdateDto.Images != null && variantUpdateDto.Images.Count > 0)
            {
                foreach (var file in variantUpdateDto.Images)
                {
                    await _imageService.UploadFileAsync(file, updatedProductVariant.Id);
                }
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductVariant(int id)
        {
            var productVariant = await _productVariantService.GetProductVariantById(id);
            if (productVariant == null)
            {
                return NotFound();
            }
            await _productVariantService.DeleteProductVariant(id);
            return Ok();
        }
        
    }
}

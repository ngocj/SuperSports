using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SP.Application.Dto.ProductDto;
using SP.Application.Service.Interface;
using SP.Domain.Entity;

namespace SP.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        public ProductController(IMapper mapper, IProductService productService)
        {
            _mapper = mapper;
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProducts();
            var productDto = _mapper.Map<IEnumerable<ProductViewDto>>(products);
            return Ok(productDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productService.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            await _productService.DeleteProduct(id);
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            var productDto = _mapper.Map<ProductViewDto>(product);
            return Ok(productDto);
        }

        [HttpGet("brand/{brandId}")]
        public async Task<IActionResult> GetAllProductsByBrandId(int brandId)
        {
            var products = await _productService.GetAllProductsByBrandId(brandId);
            var productDto = _mapper.Map<IEnumerable<ProductViewDto>>(products);
            return Ok(productDto);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetAllProductsByCategoryId(int categoryId)
        {
            var products = await _productService.GetAllByCategoryIdAsync(categoryId);
            var productDto = _mapper.Map<IEnumerable<ProductViewDto>>(products);
            return Ok(productDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductCreateDto productCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = _mapper.Map<Product>(productCreateDto);
            await _productService.CreateProduct(product); // product.Id được gán sau khi lưu

            return Ok(new { Id = product.Id });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct(ProductUpdateDto productUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var product = await _productService.GetProductById(productUpdateDto.Id);
            if (product == null)
            {
                return NotFound();
            }
            var updatedProduct = _mapper.Map<Product>(productUpdateDto);
            await _productService.UpdateProduct(updatedProduct);
            return Ok();
        }

        [HttpGet("subCategory/{subCategoryId}")]
        public async Task<IActionResult> GetAllProductsBySubCategoryId(int subCategoryId)
        {
            var products = await _productService.GetAllProductsBySubCategoryId(subCategoryId);
            var productDto = _mapper.Map<IEnumerable<ProductViewDto>>(products);
            return Ok(productDto);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetAllByCategoryAndBrand(int? SubcategoryId, int? brandId, bool? isActive)
        {
            var products = await _productService.GetAllByCategoryAndBrandAsync(SubcategoryId, brandId, isActive);
            var productDto = _mapper.Map<IEnumerable<ProductViewDto>>(products);
            return Ok(productDto);
        }

        [HttpGet("lastest")]
        public async Task<IActionResult> GetAllProductsByLastest(decimal? priceFrom, decimal? priceTo, int categoryId, int? subCategoryId, int? brandId, string? search)
        {
            var products = await _productService.GetAllByLastestAsync(priceFrom, priceTo, categoryId, subCategoryId, brandId, search);
            var productDto = _mapper.Map<IEnumerable<ProductViewDto>>(products);
            return Ok(productDto);
        }

        [HttpGet("price_desc")]
        public async Task<IActionResult> GetAllProductsByPriceDescending(decimal? priceFrom, decimal? priceTo, int categoryId, int? subCategoryId, int? brandId, string? search)
        {
            var products = await _productService.GetAllProductsByPriceDescending(priceFrom, priceTo, categoryId, subCategoryId, brandId, search);
            var productDto = _mapper.Map<IEnumerable<ProductViewDto>>(products);
            return Ok(productDto);

        }

        [HttpGet("price_asc")]
        public async Task<IActionResult> GetAllProductsByPriceAscending(decimal? priceFrom, decimal? priceTo, int categoryId, int? subCategoryId, int? brandId, string? search)
        {
            var products = await _productService.GetAllProductsByPriceAscending(priceFrom, priceTo, categoryId, subCategoryId, brandId, search);
            var productDto = _mapper.Map<IEnumerable<ProductViewDto>>(products);
            return Ok(productDto);

        }

        [HttpGet("best_selling")]
        public async Task<IActionResult> GetAllProductsByBestSelling(decimal? priceFrom, decimal? priceTo, int categoryId, int? subCategoryId, int? brandId, string? search)
        {
            var products = await _productService.GetAllProductsByBestSelling(priceFrom, priceTo, categoryId, subCategoryId, brandId, search);
            var productDto = _mapper.Map<IEnumerable<ProductViewDto>>(products);
            return Ok(productDto);

        }

        [HttpGet("top-newest")]
        public async Task<IActionResult> GetTop10Newest()
        {
            var products = await _productService.GetTop10NewestAsync();
            var productDto = _mapper.Map<IEnumerable<ProductViewDto>>(products);
            return Ok(productDto);
        }

        [HttpGet("top-best-selling")]
        public async Task<IActionResult> GetTop10BestSelling()
        {
            var products = await _productService.GetTop10BestSellingAsync();
            var productDto = _mapper.Map<IEnumerable<ProductViewDto>>(products);
            return Ok(productDto);
        }

        [HttpGet("count-by-category")]
        public async Task<IActionResult> GetProductCountByCategoryWithNames()
        {
            var result = await _productService.GetProductCountByCategoryWithNamesAsync();
            return Ok(result);
        }
        [HttpGet("count-by-brand")]
        public async Task<IActionResult> GetProductCountByBrandWithNames()
        {
            var result = await _productService.GetProductCountByBrandWithNamesAsync();
            return Ok(result);
        }
        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStockProductDetails(int threshold = 10)
        {
            var result = await _productService.GetLowStockProductDetailsAsync(threshold);
            return Ok(result);
        }
        [HttpGet("top-revenue")]
        public async Task<IActionResult> GetTopRevenueProductDetails( int topCount = 10)
        {
            var result = await _productService.GetTopRevenueProductDetailsAsync(topCount);
            return Ok(result);
        }
       

    }
}
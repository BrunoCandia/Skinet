using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(string? brand, string? type, string? sort)
        {
            var products = await _productRepository.GetProductsAsync(brand, type, sort);

            return Ok(products);
        }

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<Product>> GetProduct(Guid id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);

            if (product is null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            if (product is null)
            {
                return BadRequest();
            }

            await _productRepository.AddProductAsync(product);

            if (await _productRepository.SaveChangesAsync())
            {
                return CreatedAtAction("GetProduct", new { id = product.Id }, product);
            }

            return BadRequest("Cannot create product");
        }

        [HttpPut("{id:Guid}")]
        public async Task<ActionResult> UpdateProduct(Guid id, Product product)
        {
            if (product.Id != id || !await ProductExistsAsync(id))
            {
                return BadRequest("Cannot update this product");
            }

            _productRepository.UpdateProduct(product);

            if (await _productRepository.SaveChangesAsync())
            {
                return NoContent();
            }

            return BadRequest($"Cannot update product {product.Id}");
        }

        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult> DeleteProduct(Guid id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);

            if (product is null)
            {
                return NotFound();
            }

            _productRepository.DeleteProduct(product);

            if (await _productRepository.SaveChangesAsync())
            {
                return NoContent();
            }

            return BadRequest($"Cannot delete product {id}");
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
        {
            var brands = await _productRepository.GetBrandsAsync();

            return Ok(brands);
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
        {
            var types = await _productRepository.GetTypesAsync();

            return Ok(types);
        }

        private async Task<bool> ProductExistsAsync(Guid id)
        {
            return await _productRepository.ProductExistsAsync(id);
        }
    }
}

using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly StoreContext _storeContext;

        public ProductsController(StoreContext storeContext)
        {
            _storeContext = storeContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _storeContext.Products.ToListAsync();
        }

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<Product>> GetProduct(Guid id)
        {
            var product = await _storeContext.Products.SingleOrDefaultAsync(x => x.Id == id);

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

            _storeContext.Products.Add(product);

            await _storeContext.SaveChangesAsync();

            return Ok(product);
        }

        [HttpPut("{id:Guid}")]
        public async Task<ActionResult> UpdateProduct(Guid id, Product product)
        {
            if (product.Id != id || !await ProductExistsAsync(id))
            {
                return BadRequest("Cannot update this product");
            }

            _storeContext.Entry(product).State = EntityState.Modified;

            await _storeContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult> DeleteProduct(Guid id)
        {
            var product = await _storeContext.Products.FindAsync(id);

            if (product is null)
            {
                return NotFound();
            }

            _storeContext.Products.Remove(product);

            await _storeContext.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> ProductExistsAsync(Guid id)
        {
            return await _storeContext.Products.AnyAsync(x => x.Id == id);
        }
    }
}

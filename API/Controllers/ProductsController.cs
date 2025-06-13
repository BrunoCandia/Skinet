using API.RequestHelper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IProductRepository _productRepository;
        private readonly IGenericRepository<Product> _genericRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductsController(IProductRepository productRepository, IGenericRepository<Product> genericRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _genericRepository = genericRepository;
            _unitOfWork = unitOfWork;
        }

        [Cache(60)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts([FromQuery] ProductSpecParams productSpecParams)
        {
            var spec = new ProductFilterSortPaginationSpecification(productSpecParams);

            return await CreatePagedResult(_unitOfWork.Repository<Product>(), spec, productSpecParams.PageIndex, productSpecParams.PageSize);
        }

        [Cache(60)]
        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<Product>> GetProduct(Guid id)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);

            if (product is null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [InvalidateCache("api/products|")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            if (product is null)
            {
                return BadRequest();
            }

            await _unitOfWork.Repository<Product>().AddAsync(product);

            if (await _unitOfWork.CompleteAsync())
            {
                return CreatedAtAction("GetProduct", new { id = product.Id }, product);
            }

            return BadRequest("Cannot create product");
        }

        [InvalidateCache("api/products|")]
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:Guid}")]
        public async Task<ActionResult> UpdateProduct(Guid id, Product product)
        {
            if (product.Id != id || !await ProductExistsAsync(id))
            {
                return BadRequest("Cannot update this product");
            }

            _unitOfWork.Repository<Product>().Update(product);

            if (await _unitOfWork.CompleteAsync())
            {
                return NoContent();
            }

            return BadRequest($"Cannot update product {product.Id}");
        }

        [InvalidateCache("api/products|")]
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult> DeleteProduct(Guid id)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);

            if (product is null)
            {
                return NotFound();
            }

            _unitOfWork.Repository<Product>().Delete(product);

            if (await _unitOfWork.CompleteAsync())
            {
                return NoContent();
            }

            return BadRequest($"Cannot delete product {id}");
        }

        [Cache(10000)]
        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
        {
            var spec = new BrandListSpecification();

            var brands = await _unitOfWork.Repository<Product>().GetEntitiesWithSpecAsync(spec);

            return Ok(brands);
        }

        [Cache(10000)]
        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
        {
            var spec = new TypeListSpecification();

            var types = await _unitOfWork.Repository<Product>().GetEntitiesWithSpecAsync(spec);

            return Ok(types);
        }

        private async Task<bool> ProductExistsAsync(Guid id)
        {
            return await _unitOfWork.Repository<Product>().ExistsAsync(id);
        }

        #region Without Unit of Work

        ////[HttpGet]
        ////public async Task<ActionResult<IEnumerable<Product>>> GetProducts([FromQuery] ProductSpecParams productSpecParams)
        ////{
        ////    var spec = new ProductFilterSortPaginationSpecification(productSpecParams);

        ////    ////var products = await _genericRepository.GetEntitiesWithSpecAsync(spec);

        ////    ////var totalCount = await _genericRepository.CountAsync(spec);

        ////    ////var pagination = new Pagination<Product>(productSpecParams.PageIndex, productSpecParams.PageSize, totalCount, products);

        ////    ////return Ok(pagination);

        ////    return await CreatePagedResult(_genericRepository, spec, productSpecParams.PageIndex, productSpecParams.PageSize);
        ////}

        ////[HttpGet]
        ////public async Task<ActionResult<IEnumerable<Product>>> GetProducts(string? brand, string? type, string? sort)
        ////{
        ////    var products = await _productRepository.GetProductsAsync(brand, type, sort);

        ////    return Ok(products);
        ////}

        ////[HttpGet("{id:Guid}")]
        ////public async Task<ActionResult<Product>> GetProduct(Guid id)
        ////{
        ////    var product = await _genericRepository.GetByIdAsync(id);

        ////    if (product is null)
        ////    {
        ////        return NotFound();
        ////    }

        ////    return Ok(product);
        ////}

        ////[HttpPost]
        ////public async Task<ActionResult<Product>> CreateProduct(Product product)
        ////{
        ////    if (product is null)
        ////    {
        ////        return BadRequest();
        ////    }

        ////    await _genericRepository.AddAsync(product);

        ////    if (await _genericRepository.SaveChangesAsync())
        ////    {
        ////        return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        ////    }

        ////    return BadRequest("Cannot create product");
        ////}

        ////[HttpPut("{id:Guid}")]
        ////public async Task<ActionResult> UpdateProduct(Guid id, Product product)
        ////{
        ////    if (product.Id != id || !await ProductExistsAsync(id))
        ////    {
        ////        return BadRequest("Cannot update this product");
        ////    }

        ////    _genericRepository.Update(product);

        ////    if (await _genericRepository.SaveChangesAsync())
        ////    {
        ////        return NoContent();
        ////    }

        ////    return BadRequest($"Cannot update product {product.Id}");
        ////}

        ////[HttpDelete("{id:Guid}")]
        ////public async Task<ActionResult> DeleteProduct(Guid id)
        ////{
        ////    var product = await _genericRepository.GetByIdAsync(id);

        ////    if (product is null)
        ////    {
        ////        return NotFound();
        ////    }

        ////    _productRepository.DeleteProduct(product);

        ////    if (await _genericRepository.SaveChangesAsync())
        ////    {
        ////        return NoContent();
        ////    }

        ////    return BadRequest($"Cannot delete product {id}");
        ////}

        ////[HttpGet("brands")]
        ////public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
        ////{
        ////    var spec = new BrandListSpecification();

        ////    var brands = await _genericRepository.GetEntitiesWithSpecAsync(spec);

        ////    return Ok(brands);
        ////}

        ////[HttpGet("types")]
        ////public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
        ////{
        ////    var spec = new TypeListSpecification();

        ////    var types = await _genericRepository.GetEntitiesWithSpecAsync(spec);

        ////    return Ok(types);
        ////}

        ////[HttpGet("brands")]
        ////public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
        ////{
        ////    var brands = await _productRepository.GetBrandsAsync();

        ////    return Ok(brands);
        ////}

        ////[HttpGet("types")]
        ////public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
        ////{
        ////    var types = await _productRepository.GetTypesAsync();

        ////    return Ok(types);
        ////}

        ////private async Task<bool> ProductExistsAsync(Guid id)
        ////{
        ////    return await _genericRepository.ExistsAsync(id);
        ////}

        #endregion
    }
}

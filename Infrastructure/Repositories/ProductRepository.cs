using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly StoreContext _storeContext;

        public ProductRepository(StoreContext storeContext)
        {
            _storeContext = storeContext;
        }

        public async Task AddProductAsync(Product product)
        {
            await _storeContext.Products.AddAsync(product);
        }

        public void DeleteProduct(Product product)
        {
            _storeContext.Products.Remove(product);
        }

        public async Task<IReadOnlyList<string>> GetBrandsAsync()
        {
            return await _storeContext.Products.Select(p => p.Brand)
                .Distinct()
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(Guid id)
        {
            return await _storeContext.Products.FindAsync(id);
        }

        public async Task<IReadOnlyList<Product>> GetProductsAsync(string? brand, string? type, string? sort)
        {
            var query = _storeContext.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(brand))
            {
                query = query.Where(b => b.Brand == brand);
            }

            if (!string.IsNullOrWhiteSpace(type))
            {
                query = query.Where(b => b.Type == type);
            }

            query = sort switch
            {
                "priceAsc" => query.OrderBy(x => x.Price),
                "priceDesc" => query.OrderByDescending(x => x.Price),
                _ => query.OrderBy(x => x.Name)
            };

            return await query.ToListAsync();
        }

        public async Task<IReadOnlyList<string>> GetTypesAsync()
        {
            return await _storeContext.Products.Select(p => p.Type)
                .Distinct()
                .ToListAsync();
        }

        public async Task<bool> ProductExistsAsync(Guid id)
        {
            return await _storeContext.Products.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _storeContext.SaveChangesAsync() > 0;
        }

        public void UpdateProduct(Product product)
        {
            ////_storeContext.Entry(product).State = EntityState.Modified;

            _storeContext.Products.Update(product);
        }
    }
}

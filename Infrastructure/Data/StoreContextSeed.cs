using Core.Entities;
using System.Text.Json;

namespace Infrastructure.Data
{
    public static class StoreContextSeed
    {
        public static async Task SeedAsync(StoreContext storeContext)
        {
            if (!storeContext.Products.Any())
            {
                var productsData = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/products.json");

                var products = JsonSerializer.Deserialize<List<Product>>(productsData);

                if (products is null)
                {
                    return;
                }

                await storeContext.Products.AddRangeAsync(products);

                await storeContext.SaveChangesAsync();
            }
        }
    }
}

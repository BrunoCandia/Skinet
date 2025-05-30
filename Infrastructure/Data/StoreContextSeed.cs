using Core.Entities;
using System.Reflection;
using System.Text.Json;

namespace Infrastructure.Data
{
    public static class StoreContextSeed
    {
        public static async Task SeedAsync(StoreContext storeContext)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (!storeContext.Products.Any())
            {
                var productsData = await File.ReadAllTextAsync(path + @"/Data/SeedData/products.json");

                var products = JsonSerializer.Deserialize<List<Product>>(productsData);

                if (products is null)
                {
                    return;
                }

                await storeContext.Products.AddRangeAsync(products);

                await storeContext.SaveChangesAsync();
            }

            if (!storeContext.DeliveryMethods.Any())
            {
                var deliveryMethodsData = await File.ReadAllTextAsync(path + @"/Data/SeedData/delivery.json");

                var deliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryMethodsData);

                if (deliveryMethods is null)
                {
                    return;
                }

                await storeContext.DeliveryMethods.AddRangeAsync(deliveryMethods);

                await storeContext.SaveChangesAsync();
            }
        }
    }
}

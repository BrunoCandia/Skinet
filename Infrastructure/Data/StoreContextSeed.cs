using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json;

namespace Infrastructure.Data
{
    public static class StoreContextSeed
    {
        public static async Task SeedAsync(StoreContext storeContext, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                Console.WriteLine("Seeding roles");
                await roleManager.CreateAsync(new IdentityRole("Admin"));
                await roleManager.CreateAsync(new IdentityRole("Customer"));
            }

            if (!await userManager.Users.AnyAsync(x => x.UserName == "admin@test.com"))
            {
                Console.WriteLine("Seeding users");

                var user = new User
                {
                    UserName = "admin@test.com",
                    Email = "admin@test.com",
                };

                await userManager.CreateAsync(user, "Pa$$w0rd");

                await userManager.AddToRoleAsync(user, "Admin");
            }

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (!await storeContext.Products.AnyAsync())
            {
                Console.WriteLine("Seeding products");

                var productsData = await File.ReadAllTextAsync(path + @"/Data/SeedData/products.json");

                var products = JsonSerializer.Deserialize<List<Product>>(productsData);

                if (products is null)
                {
                    return;
                }

                await storeContext.Products.AddRangeAsync(products);

                await storeContext.SaveChangesAsync();
            }

            if (!await storeContext.DeliveryMethods.AnyAsync())
            {
                Console.WriteLine("Seeding delivery methods");

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

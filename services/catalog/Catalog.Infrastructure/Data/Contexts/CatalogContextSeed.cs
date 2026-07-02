using Catalog.Core.Entities;
using MongoDB.Driver;
using System.Text.Json;

namespace Catalog.Infrastructure.Data.Contexts
{
    public static class CatalogContextSeed
    {
        public static async Task SeedDataAsync(IMongoCollection<Product> productCollection)
        {
            var hasProducts = await productCollection.Find(_ => true).AnyAsync();
            if (hasProducts)
                return;

            var filePath = Path.Combine("Data", "SeedData", "products.json");

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Seed file not exists: {filePath}");
                return;
            }

            try
            {
                var productData = await File.ReadAllTextAsync(filePath);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var products = JsonSerializer.Deserialize<List<Product>>(productData, options);

                if (products is not null && products.Any())
                {
                    await productCollection.InsertManyAsync(products);
                    Console.WriteLine($"Seeded {products.Count} products.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to seed products: {ex.Message}");
            }
        }
    }
}

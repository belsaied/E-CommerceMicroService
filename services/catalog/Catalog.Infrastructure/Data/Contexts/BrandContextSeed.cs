using Catalog.Core.Entities;
using MongoDB.Driver;
using System.Text.Json;

namespace Catalog.Infrastructure.Data.Contexts
{
    public static class BrandContextSeed
    {
        public static async Task SeedDataAsync(IMongoCollection<ProductBrand> brandCollection)
        {
            var hasBrands = await brandCollection.Find(_ => true).AnyAsync();
            if (hasBrands)
                return;

            var filePath = Path.Combine("Data", "SeedData", "brands.json");

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Seed file not exists: {filePath}");
                return;
            }

            try
            {
                var brandData = await File.ReadAllTextAsync(filePath);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandData, options);

                if (brands is not null && brands.Any())
                {
                    await brandCollection.InsertManyAsync(brands);
                    Console.WriteLine($"Seeded {brands.Count} brands.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to seed brands: {ex.Message}");
            }
        }
    }
}

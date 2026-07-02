using Catalog.Core.Entities;
using MongoDB.Driver;
using System.Text.Json;

namespace Catalog.Infrastructure.Data.Contexts
{
    public static class TypeContextSeed
    {
        public static async Task SeedDataAsync(IMongoCollection<ProductType> typeCollection)
        {
            var hasTypes = await typeCollection.Find(_ => true).AnyAsync();
            if (hasTypes)
                return;

            var filePath = Path.Combine("Data", "SeedData", "types.json");

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Seed file not exists: {filePath}");
                return;
            }

            try
            {
                var typeData = await File.ReadAllTextAsync(filePath);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var types = JsonSerializer.Deserialize<List<ProductType>>(typeData, options);

                if (types is not null && types.Any())
                {
                    await typeCollection.InsertManyAsync(types);
                    Console.WriteLine($"Seeded {types.Count} types.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to seed types: {ex.Message}");
            }
        }
    }
}

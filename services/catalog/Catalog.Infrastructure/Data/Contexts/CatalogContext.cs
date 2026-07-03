using Catalog.Core.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Data.Contexts
{
    public class CatalogContext : ICatalogContext
    {
        public IMongoCollection<Product> Products { get; }

        public IMongoCollection<ProductBrand> Brands { get; }

        public IMongoCollection<ProductType> Types {  get; }
        public CatalogContext(IConfiguration configuration)
        {
            // Create a MongoClient using the connection string from the configuration
            var client = new MongoClient(configuration["DatabaseSettings:ConnectionString"]);
            var database = client.GetDatabase(configuration["DatabaseSettings:DatabaseName"]);

            // Get the collections from the database using the collection names from the configuration
            Products = database.GetCollection<Product>(configuration["DatabaseSettings:ProductsCollection"]);
            Brands = database.GetCollection<ProductBrand>(configuration["DatabaseSettings:BrandsCollection"]);
            Types = database.GetCollection<ProductType>(configuration["DatabaseSettings:TypesCollection"]);

            // Seed Data
            _ = CatalogContextSeed.SeedDataAsync(Products);
            _ = BrandContextSeed.SeedDataAsync(Brands);
            _ = TypeContextSeed.SeedDataAsync(Types);
        }
    }
}

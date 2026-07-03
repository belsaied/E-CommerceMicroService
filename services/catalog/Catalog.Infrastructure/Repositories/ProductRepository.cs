using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Catalog.Infrastructure.Data.Contexts;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository, IBrandRepository, ITypeRepository
    {
        public ICatalogContext _context { get; set; }
        public ProductRepository(ICatalogContext context)
        {
            _context = context;
        }
        public async Task<Product> GetProductById(string id)
             => await _context.Products.Find(p => p.Id == id).FirstOrDefaultAsync();

        public async Task<IEnumerable<Product>> GetAllProducts()
             => await _context.Products.Find(p => true).ToListAsync();

        public async Task<IEnumerable<Product>> GetAllProductsByBrand(string name)
            => await _context.Products.Find(p => p.Brand.Name == name).ToListAsync();

        public async Task<IEnumerable<Product>> GetAllProductsByName(string name)
            => await _context.Products.Find(p => p.Name == name).ToListAsync();

        public async Task<Product> CreateProduct(Product product)
        {
           await _context.Products.InsertOneAsync(product);
            return product;
        }

        public async Task<bool> DeleteProduct(string id)
        {
            var deletedProduct = await _context.Products.DeleteOneAsync(p => p.Id == id);
            return deletedProduct.IsAcknowledged && deletedProduct.DeletedCount > 0;
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            var updatedProduct = await _context.Products.ReplaceOneAsync(p => p.Id == product.Id, product);
            return updatedProduct.IsAcknowledged && updatedProduct.ModifiedCount > 0;
        }

        public async Task<IEnumerable<ProductBrand>> GetAllBrands()
            => await _context.Brands.Find(b => true).ToListAsync();

        public async Task<IEnumerable<ProductType>> GetAllTypes()
            => await _context.Types.Find(t => true).ToListAsync();

    }
}

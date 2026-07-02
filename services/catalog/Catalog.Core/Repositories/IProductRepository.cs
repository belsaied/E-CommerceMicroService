using Catalog.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Core.Repositories
{
    public interface IProductRepository
    {
        // GetAll Products
        Task<IEnumerable<Product>> GetAllProducts();
        // Get Product By Id
        Task<Product> GetProductById(string id);
        // Get Products By Name
        Task<IEnumerable<Product>> GetAllProductsByName(string name);
        // Get All Products By Brand
        Task<IEnumerable<Product>> GetAllProductsByBrand(string name);
        // Create Product
        Task<Product> CreateProduct(Product product);
        // Update Product
        Task<bool> UpdateProduct(Product product);
        // Delete Product
        Task<bool> DeleteProduct(string id);
    }
}

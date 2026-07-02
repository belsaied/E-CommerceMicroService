using Catalog.Core.Entities;

namespace Catalog.Core.Repositories
{
    public interface IBrandRepository
    {
        // GetAll Brands
        Task<IEnumerable<ProductBrand>> GetAllBrands();
    }
}

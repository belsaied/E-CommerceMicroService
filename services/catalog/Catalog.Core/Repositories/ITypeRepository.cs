using Catalog.Core.Entities;

namespace Catalog.Core.Repositories
{
    public interface ITypeRepository
    {
        // GetAll Types
        Task<IEnumerable<ProductType>> GetAllTypes();
    }
}

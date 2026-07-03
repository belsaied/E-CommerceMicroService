using Catalog.Application.Responses;
using MediatR;

namespace Catalog.Application.Queries
{
    public class GetAllProductsByBrandQuery : IRequest<IList<ProductResponseDto>>
    {
        public string BrandName { get; set; }
        public GetAllProductsByBrandQuery(string brandName)
        {
            BrandName = brandName;
        }  
    }
}

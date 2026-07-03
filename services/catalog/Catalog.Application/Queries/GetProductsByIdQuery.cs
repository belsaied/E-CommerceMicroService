using Catalog.Application.Responses;
using MediatR;

namespace Catalog.Application.Queries
{
    public class GetProductsByIdQuery : IRequest<ProductResponseDto>
    {
        public string Id { get; set; }
        public GetProductsByIdQuery(string id)
        {
            Id = id;
        }
    }
}

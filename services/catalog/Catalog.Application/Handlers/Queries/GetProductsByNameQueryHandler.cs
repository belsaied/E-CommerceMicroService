using AutoMapper;
using Catalog.Application.Queries;
using Catalog.Application.Responses;
using Catalog.Core.Repositories;
using MediatR;

namespace Catalog.Application.Handlers.Queries
{
    public class GetProductsByNameQueryHandler : IRequestHandler<GetProductsByNameQuery,IList<ProductResponseDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        public GetProductsByNameQueryHandler(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<IList<ProductResponseDto>> Handle(GetProductsByNameQuery request, CancellationToken cancellationToken)
        {
            var productList = await _productRepository.GetAllProductsByName(request.Name);
            var productListResponse = _mapper.Map<IList<ProductResponseDto>>(productList);
            return productListResponse;
        }
    }
}

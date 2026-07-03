using AutoMapper;
using Catalog.Application.Queries;
using Catalog.Application.Responses;
using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using MediatR;

namespace Catalog.Application.Handlers.Queries
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IList<ProductResponseDto>>
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;
        public GetAllProductsQueryHandler(IMapper mapper, IProductRepository productRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
        }
        public async Task<IList<ProductResponseDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var productList = await _productRepository.GetAllProducts();
            var productResponseList = _mapper.Map<IList<Product>,IList<ProductResponseDto>>(productList.ToList());
            return productResponseList;
        }
    }
}

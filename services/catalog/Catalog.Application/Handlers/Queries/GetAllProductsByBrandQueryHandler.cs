using AutoMapper;
using Catalog.Application.Queries;
using Catalog.Application.Responses;
using Catalog.Core.Repositories;
using MediatR;

namespace Catalog.Application.Handlers.Queries
{
    public class GetAllProductsByBrandQueryHandler : IRequestHandler<GetAllProductsByBrandQuery, IList<ProductResponseDto>>
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;
        public GetAllProductsByBrandQueryHandler(IMapper mapper, IProductRepository productRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
        }
        public async Task<IList<ProductResponseDto>> Handle(GetAllProductsByBrandQuery request, CancellationToken cancellationToken)
        {
            var productList = await _productRepository.GetAllProductsByBrand(request.BrandName);
            var productListResponse = _mapper.Map<IList<ProductResponseDto>>(productList);
            return productListResponse;
        }
    }
}

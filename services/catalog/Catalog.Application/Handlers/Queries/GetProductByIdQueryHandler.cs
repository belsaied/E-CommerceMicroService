using AutoMapper;
using Catalog.Application.Queries;
using Catalog.Application.Responses;
using Catalog.Core.Repositories;
using MediatR;

namespace Catalog.Application.Handlers.Queries
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductsByIdQuery, ProductResponseDto>
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;
        public GetProductByIdQueryHandler(IMapper mapper,IProductRepository productRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
        }
        public async Task<ProductResponseDto> Handle(GetProductsByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetProductById(request.Id);
            var productResponse = _mapper.Map<ProductResponseDto>(product);
            return productResponse;
        }
    }
}

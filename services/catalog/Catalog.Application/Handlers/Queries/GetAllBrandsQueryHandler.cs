using AutoMapper;
using Catalog.Application.Queries;
using Catalog.Application.Responses;
using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using MediatR;

namespace Catalog.Application.Handlers.Queries
{
    public class GetAllBrandsQueryHandler : IRequestHandler<GetAllBrandsQuery, IList<BrandResponseDto>>
    {
        private readonly IMapper _mapper;
        private readonly IBrandRepository _brandRepository;
        public GetAllBrandsQueryHandler(IMapper mapper, IBrandRepository brandRepository)
        {
            _mapper = mapper;
            _brandRepository = brandRepository;
        }
        public async Task<IList<BrandResponseDto>> Handle(GetAllBrandsQuery request, CancellationToken cancellationToken)
        {
            var brandList = await _brandRepository.GetAllBrands();
            var brandResponseList = _mapper.Map<IList<ProductBrand>,IList<BrandResponseDto>>(brandList.ToList());
            return brandResponseList;
        }
    }
}

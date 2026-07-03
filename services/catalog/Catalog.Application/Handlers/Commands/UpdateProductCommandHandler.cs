using Catalog.Application.Commands;
using Catalog.Core.Repositories;
using MediatR;

namespace Catalog.Application.Handlers.Commands
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
    {
        private readonly IProductRepository _productRepository;
        public UpdateProductCommandHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var productEntity = await _productRepository.UpdateProduct(new Core.Entities.Product
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description,
                ImageFile = request.ImageFile,
                Price = request.Price,
                summary = request.summary,
                Brand = request.Brand,
                Type = request.Type,
            });
            return productEntity;
        }
    }
}

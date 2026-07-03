using Catalog.Application.Responses;
using Catalog.Core.Entities;
using MediatR;
using MongoDB.Bson.Serialization.Attributes;

namespace Catalog.Application.Commands
{
    public class CreateProductCommand : IRequest<ProductResponseDto>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string summary { get; set; }
        public string ImageFile { get; set; }
        [BsonRepresentation(MongoDB.Bson.BsonType.Decimal128)]
        public decimal Price { get; set; }
        public ProductBrand Brand { get; set; }
        public ProductType Type { get; set; }
    }
}

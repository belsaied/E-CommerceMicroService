using Catalog.Core.Entities;
using MongoDB.Bson.Serialization.Attributes;

namespace Catalog.Application.Responses
{
    public class ProductResponseDto
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
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

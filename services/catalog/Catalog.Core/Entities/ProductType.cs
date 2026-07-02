using MongoDB.Bson.Serialization.Attributes;

namespace Catalog.Core.Entities
{
    public class ProductType : BaseEntity
    {
       // [BsonElement("TypeName")]
        public string Name { get; set; }
    }
}

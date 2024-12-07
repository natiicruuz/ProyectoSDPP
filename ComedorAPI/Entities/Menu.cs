using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace ComedorAPI.Entities
{
    public class Menu
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required]
        [BsonElement("name")]
        public string Name { get; set; }

        [Required]
        [BsonElement("description")]
        public string Description { get; set; }

        [Required]
        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("available")]
        public bool Available { get; set; }
    }
}

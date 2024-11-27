using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ComedorAPI.Entities
{
    public class Reservation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } 

        [BsonElement("menu")]
        public string MenuId { get; set; } 

        [BsonElement("fechaReserva")]
        public DateTime FechaReserva { get; set; }

        [BsonElement("estadoReserva")]
        public bool EstadoReserva { get; set; } 

        [BsonElement("comida")]
        public string Comida { get; set; }
    }
}

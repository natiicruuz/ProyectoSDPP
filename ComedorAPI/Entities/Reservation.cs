using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

public class Reservation
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }  // MongoDB _id

    [BsonRepresentation(BsonType.ObjectId)]
    public string? MenuId { get; set; } // Referencia al _id del menú

    public DateTime FechaReserva { get; set; }
    public bool EstadoReserva { get; set; }
}

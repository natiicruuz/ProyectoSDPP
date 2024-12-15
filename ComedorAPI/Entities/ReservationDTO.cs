using MongoDB.Bson.Serialization.Attributes;

namespace ComedorAPI.Entities
{
    public class ReservationDTO
    {
        [BsonIgnoreIfNull]
        public string? Id { get; set; }
        public Menu? Menu { get; set; }
        public DateTime FechaReserva { get; set; }
        public bool EstadoReserva { get; set; }
    }
}

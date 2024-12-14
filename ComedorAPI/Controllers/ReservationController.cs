using ComedorAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ComedorAPI.Data;
using ComedorAPI.Entities;
using MongoDB.Bson;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ComedorAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ReservationController : ControllerBase
    {

        private readonly IMongoCollection<Reservation> _reservations;
        private readonly IMongoCollection<Menu> _menus; // Colección de menús para validar el ID del menú.


        public ReservationController(MongoDbService mongoDbService)
        {
            _reservations = mongoDbService.Database?.GetCollection<Reservation>("reservation");
            _menus = mongoDbService.Database?.GetCollection<Menu>("menu");

        }
        // GET: api/<ReservationController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> Get()
        {
            var reservations = await _reservations.Find(FilterDefinition<Reservation>.Empty).ToListAsync();

            // Anidar el objeto Menu
            var reservationsWithMenus = new List<ReservationDTO>();

            foreach (var reservation in reservations)
            {
                var menu = await _menus.Find(m => m.Id == reservation.MenuId).FirstOrDefaultAsync();

                reservationsWithMenus.Add(new ReservationDTO
                {
                    Id = reservation.Id,
                    Menu = menu,
                    FechaReserva = reservation.FechaReserva,
                    EstadoReserva = reservation.EstadoReserva
                });
            }

            return Ok(reservationsWithMenus);
        }


        // GET api/<ReservationController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetById(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return BadRequest(new { message = "El ID de la reserva no es válido" });
            }

            var reservation = await _reservations.Find(r => r.Id == id).FirstOrDefaultAsync();

            if (reservation == null)
            {
                return NotFound(new { message = "Reserva no encontrada" });
            }

            return Ok(reservation);
        }

        // POST api/<ReservationController>
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Reservation reservation)
        {
            try
            {
                reservation.Id = null; // MongoDB generará el ID automáticamente.
                
 
                var menu = await _menus.Find(m => m.Id == id).FirstOrDefaultAsync();
                if (menu == null)
                {
                    return NotFound(new { message = $"No se encontró un menú con el ID: {reservation.MenuId}" });
                }

                // Inserta la reserva
                await _reservations.InsertOneAsync(reservation);

                // Construye la respuesta
                var response = new
                {
                    Id = reservation.Id,
                    FechaReserva = reservation.FechaReserva,
                    EstadoReserva = reservation.EstadoReserva,
                    Menu = menu
                };

                return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error interno: {ex.Message}" });
            }
        }


        // PUT api/<ReservationController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, Reservation updatedReservation)
        {
            try
            {
 
                var filter = Builders<Reservation>.Filter.Eq(r => r.Id, id);
                var result = await _reservations.ReplaceOneAsync(filter, updatedReservation);

                if (result.MatchedCount == 0)
                {
                    return NotFound($"No se encontró la reserva con ID: {id}");
                }

                return Ok("Reserva actualizada correctamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar la reserva: {ex.Message}");
            }
        }

        // DELETE api/<ReservationController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var filter = Builders<Reservation>.Filter.Eq(x => x.Id, id);
            var result = await _reservations.DeleteOneAsync(filter);
            if (result.DeletedCount == 0)
            {
                return NotFound($"No se encontró la reserva con ID: {id}");
            }
            return Ok();
        }
    }
}

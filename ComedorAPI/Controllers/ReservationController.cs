using ComedorAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ComedorAPI.Data;
using ComedorAPI.Entities;

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
        public async Task<IEnumerable<Reservation>> Get()
        {
            return await _reservations.Find(FilterDefinition<Reservation>.Empty).ToListAsync(); 
        }

        // GET api/<ReservationController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation?>> GetById(string id)
        {
            var filter = Builders<Reservation>.Filter.Eq(x => x.Id, id);
            var reservation = await _reservations.Find(filter).FirstOrDefaultAsync();
            return reservation is not null ? Ok(reservation) : NotFound();
        }

        // POST api/<ReservationController>
        [HttpPost]
        public async Task<ActionResult> Create(Reservation reservation)
        {
            try
            {
                // Garantizar que el ID sea nulo para que MongoDB lo genere automáticamente
                reservation.Id = null;

                await _reservations.InsertOneAsync(reservation);
                return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, reservation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear la reserva: {ex.Message}");
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

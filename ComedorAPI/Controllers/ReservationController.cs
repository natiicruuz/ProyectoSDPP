using ComedorAPI.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ComedorAPI.Data;
using ComedorAPI.Services;
using MongoDB.Bson;

namespace ComedorAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly IMongoCollection<Reservation> _reservations;
        private readonly MenuService _menuService;

        public ReservationController(MongoDbService mongoDbService, MenuService menuService)
        {
            _reservations = mongoDbService.Database?.GetCollection<Reservation>("reservation");
            _menuService = menuService;
        }

        // GET: api/<ReservationController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> Get()
        {
            var reservations = await _reservations.Find(FilterDefinition<Reservation>.Empty).ToListAsync();

            var reservationsWithMenus = new List<ReservationDTO>();

            foreach (var reservation in reservations)
            {
                var menu = await _menuService.GetMenuById(reservation.MenuId);
                if (menu == null)
                {
                    return NotFound($"Menú con ID {reservation.MenuId} no encontrado.");
                }

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
        public async Task<ActionResult<ReservationDTO>> GetById(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return BadRequest(new { message = "El ID de la reserva no es válido" });
            }

            var reservation = await _reservations.Find(r => r.Id == id).FirstOrDefaultAsync();
            if (reservation == null)
            {
                return NotFound($"Reserva con ID {id} no encontrada.");
            }

            var menu = await _menuService.GetMenuById(reservation.MenuId);
            if (menu == null)
            {
                return NotFound($"Menú con ID {reservation.MenuId} no encontrado.");
            }

            return Ok(new ReservationDTO
            {
                Id = reservation.Id,
                Menu = menu,
                FechaReserva = reservation.FechaReserva,
                EstadoReserva = reservation.EstadoReserva
            });
        }

        // POST api/<ReservationController>
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] Reservation reservation)
        {
            try
            {
                reservation.Id = null;

                var menu = await _menuService.GetMenuById(reservation.MenuId);
                if (menu == null)
                {
                    return NotFound($"Menú con ID {reservation.MenuId} no encontrado.");
                }

                await _reservations.InsertOneAsync(reservation);

                return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, new ReservationDTO
                {
                    Id = reservation.Id,
                    Menu = menu,
                    FechaReserva = reservation.FechaReserva,
                    EstadoReserva = reservation.EstadoReserva
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        // PUT api/<ReservationController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, [FromBody] Reservation updatedReservation)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return BadRequest(new { message = "El ID de la reserva no es válido" });
            }

            if (string.IsNullOrEmpty(updatedReservation.MenuId) || !ObjectId.TryParse(updatedReservation.MenuId, out _))
            {
                return BadRequest(new { message = "El ID del menú no es válido o está vacío." });
            }

            // Verificar que el menú existe
            var menu = await _menuService.GetMenuById(updatedReservation.MenuId);
            if (menu == null)
            {
                return NotFound(new { message = $"Menú con ID {updatedReservation.MenuId} no encontrado." });
            }

            // Buscar y actualizar la reserva
            var filter = Builders<Reservation>.Filter.Eq(r => r.Id, id);
            var update = Builders<Reservation>.Update
                .Set(r => r.MenuId, updatedReservation.MenuId)
                .Set(r => r.FechaReserva, updatedReservation.FechaReserva)
                .Set(r => r.EstadoReserva, updatedReservation.EstadoReserva);

            var result = await _reservations.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
            {
                return NotFound(new { message = $"Reserva con ID {id} no encontrada." });
            }

            // Retornar la reserva actualizada con el menú anidado
            return Ok(new ReservationDTO
            {
                Id = id,
                Menu = menu,
                FechaReserva = updatedReservation.FechaReserva,
                EstadoReserva = updatedReservation.EstadoReserva
            });
        }

        // DELETE api/<ReservationController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return BadRequest(new { message = "El ID de la reserva no es válido" });
            }

            var filter = Builders<Reservation>.Filter.Eq(x => x.Id, id);
            var result = await _reservations.DeleteOneAsync(filter);

            if (result.DeletedCount == 0)
            {
                return NotFound($"No se encontró la reserva con ID {id}.");
            }

            return Ok(new { message = "Reserva eliminada correctamente." });
        }
    }
}

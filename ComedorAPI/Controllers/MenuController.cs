using ComedorAPI.Entities;
using ComedorAPI.Data;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace ComedorAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuController : ControllerBase
    {
        private readonly IMongoCollection<Menu> _menus;

        public MenuController(MongoDbService mongoDbService)
        {
            _menus = mongoDbService.Database?.GetCollection<Menu>("menus");
        }

        // POST api/menu
        [HttpPost]
        public async Task<ActionResult> Create(Menu menu)
        {
            try
            {
                // Verificar si el menú ya existe
                var existingMenu = await _menus.Find(m => m.Name == menu.Name).FirstOrDefaultAsync();
                if (existingMenu != null)
                {
                    return Conflict(new { message = "El menú ya existe" });
                }

                // Guardar el nuevo menú
                await _menus.InsertOneAsync(menu);
                return CreatedAtAction(nameof(GetById), new { id = menu.Id }, menu);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error interno: {ex.Message}" });
            }
        }

        // GET api/menu/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Menu>> GetById(string id)
        {
            var menu = await _menus.Find(m => m.Id == id).FirstOrDefaultAsync();
            return menu != null ? Ok(menu) : NotFound(new { message = "Menú no encontrado" });
        }
    }
}

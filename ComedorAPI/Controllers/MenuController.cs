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
                var existingMenu = await _menus.Find(m => m.Name == menu.Name).FirstOrDefaultAsync();
                if (existingMenu != null)
                {
                    return Conflict(new { message = "El menú ya existe" });
                }

                await _menus.InsertOneAsync(menu);
                return CreatedAtAction(nameof(GetById), new { id = menu.Id }, menu);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error interno: {ex.Message}" });
            }
        }

        // GET api/menu
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Menu>>> GetAll()
        {
            try
            {
                var menus = await _menus.Find(_ => true).ToListAsync();
                return Ok(new { message = "Menús obtenidos exitosamente", data = menus });
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

        // PUT api/menu/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, Menu updatedMenu)
        {
            try
            {
                var menu = await _menus.Find(m => m.Id == id).FirstOrDefaultAsync();
                if (menu == null)
                {
                    return NotFound(new { message = "Menú no encontrado" });
                }

                var updateDefinition = Builders<Menu>.Update
                    .Set(m => m.Name, updatedMenu.Name ?? menu.Name)
                    .Set(m => m.Description, updatedMenu.Description ?? menu.Description)
                    .Set(m => m.Price, updatedMenu.Price != 0 ? updatedMenu.Price : menu.Price)
                    .Set(m => m.Available, updatedMenu.Available);

                await _menus.UpdateOneAsync(m => m.Id == id, updateDefinition);

                return Ok(new { message = "Menú actualizado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error interno: {ex.Message}" });
            }
        }

        // DELETE api/menu/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                var result = await _menus.DeleteOneAsync(m => m.Id == id);
                if (result.DeletedCount == 0)
                {
                    return NotFound(new { message = "Menú no encontrado" });
                }

                return Ok(new { message = "Menú eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error interno: {ex.Message}" });
            }
        }
    }
}

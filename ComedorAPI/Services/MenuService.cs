using ComedorAPI.Data;
using ComedorAPI.Entities;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace ComedorAPI.Services
{
    public class MenuService
    {
        private readonly IMongoCollection<Menu> _menus;

        public MenuService(MongoDbService mongoDbService)
        {
            _menus = mongoDbService.Database?.GetCollection<Menu>("menus");
        }

        public async Task<Menu> GetMenuById(string menuId)
        {
            return await _menus.Find(m => m.Id == menuId).FirstOrDefaultAsync();
        }
    }
}

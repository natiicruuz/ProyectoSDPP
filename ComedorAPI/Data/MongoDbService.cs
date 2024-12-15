using MongoDB.Driver;

namespace ComedorAPI.Data
{
    public class MongoDbService
    {

        private readonly IConfiguration _configuration;
        private readonly IMongoDatabase? _database;

        public MongoDbService(IConfiguration configuration)
        {
            try
            {
                _configuration = configuration;
                var connectionString = _configuration.GetConnectionString("DbConnection");
                var mongoUrl = MongoUrl.Create(connectionString);
                var mongoClient = new MongoClient(mongoUrl);
                _database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error conectando a la base de datos: {ex.Message}");
            }
        }

        public IMongoDatabase? Database => _database;

    }
}

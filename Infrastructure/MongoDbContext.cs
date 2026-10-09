using MongoDB.Driver;
using Microsoft.Extensions.Options;
using KlingelnbergMachineManagement.Domain.Models;
using KlingelnbergMachineManagement.Infrastructure.Options;

namespace KlingelnbergMachineManagement.Infrastructure.Persistence
{
    public class MongoDbContext
    {
        public IMongoCollection<MachineAssetMapping> MachineAssetMappings { get; }
        
        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var mongoSettings = settings.Value;
            var client = new MongoClient(mongoSettings.ConnectionString);
            var database = client.GetDatabase(mongoSettings.DatabaseName);

            MachineAssetMappings = database.GetCollection<MachineAssetMapping>(
                mongoSettings.CollectionName
            );
        }

    }
}

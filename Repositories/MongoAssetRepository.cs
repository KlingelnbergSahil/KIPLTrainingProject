using MongoDB.Driver;
using KlingelnbergMachineManagement.Domain.Interfaces;
using KlingelnbergMachineManagement.Domain.Models;
using KlingelnbergMachineManagement.Infrastructure.Persistence;

namespace KlingelnbergMachineManagement.Infrastructure.Repositories
{
    public class MongoAssetRespository : IAssetRepository , IAssetWriteRepository
    {
        public readonly IMongoCollection<MachineAssetMapping> _collection;

        public MongoAssetRespository(MongoDbContext context)
        {
            _collection = context.MachineAssetMappings;
        }

        public async Task<IEnumerable<MachineAssetMapping>> GetAllMappingAsync()
        {
            return await _collection
                .Find(FilterDefinition<MachineAssetMapping>.Empty)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetAllMachineTypesAsync()
        {
            return await _collection
                .Distinct<string>(
                    nameof(MachineAssetMapping.MachineType),
                    FilterDefinition<MachineAssetMapping>.Empty
                ).ToListAsync();
        }

        public async Task<IEnumerable<string>> GetAllAssetNamesAsync()
        {
            return await _collection
                .Distinct<string>(
                    nameof(MachineAssetMapping.AssetName),
                    FilterDefinition<MachineAssetMapping>.Empty
                ).ToListAsync();
        }

        public async Task ReplaceAllAsync(IEnumerable<MachineAssetMapping> mappings)
        {
            await _collection.DeleteManyAsync(FilterDefinition<MachineAssetMapping>.Empty);
            await _collection.InsertManyAsync(mappings);
        }

        public async Task AppendAsync(IEnumerable<MachineAssetMapping> mappings)
        {
            await _collection.InsertManyAsync(mappings);
        }
    }
}
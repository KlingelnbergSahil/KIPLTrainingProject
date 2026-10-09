using KlingelnbergMachineManagement.Domain.Interfaces;
using KlingelnbergMachineManagement.Domain.Models;

namespace KlingelnbergMachineManagement.Infrastructure.Repositories
{
    public class FileAssetWriteRepository : IAssetWriteRepository
    {
        private readonly string _dataFilePath;

        public FileAssetWriteRepository(string dataFilePath)
        {
            _dataFilePath = dataFilePath;
        }

        public async Task ReplaceAllAsync(IEnumerable<MachineAssetMapping> mappings)
        {
            var lines = mappings.Select(m =>
                $"{m.MachineType},{m.AssetName},{m.SeriesNumber}");

            await File.WriteAllLinesAsync(_dataFilePath, lines);
        }

        public async Task AppendAsync(IEnumerable<MachineAssetMapping> mappings)
        {
            var lines = mappings.Select(m =>
                $"{m.MachineType},{m.AssetName},{m.SeriesNumber}");

            await File.AppendAllLinesAsync(_dataFilePath, lines);
        }
    }
}

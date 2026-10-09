using KlingelnbergMachineManagement.Application.DTOs;
using KlingelnbergMachineManagement.Domain.Interfaces;
using System.Linq;
namespace KlingelnbergMachineManagement.Application.Services
{
    public class MachineService : IMachineService
    {
        private readonly IAssetRepository _repository;

        public MachineService(IAssetRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));         
        }

        public async Task<IEnumerable<AssetWithSeriesDto>> GetAssetsByMachineTypeAsync(string machineType)
        {
            if (string.IsNullOrWhiteSpace(machineType))
                throw new ArgumentException("Machine type cannot be empty", nameof(machineType));

            var mappings = await _repository.GetAllMappingAsync();
            return mappings
                .Where(m => m.MachineType.Equals(machineType, StringComparison.OrdinalIgnoreCase))
                .Select(m => new AssetWithSeriesDto
                {
                    AssetName = m.AssetName,
                    SeriesNumber = m.SeriesNumber
                })
                .ToList();
        }   

        public async Task<IEnumerable<string>> GetMachinesByAssetNameAsync(string assetName)
        {
            if (string.IsNullOrWhiteSpace(assetName))
                throw new ArgumentException("Asset name cannot be empty", nameof(assetName));

            var mappings = await _repository.GetAllMappingAsync();
            return mappings
                .Where(m => m.AssetName.Equals(assetName, StringComparison.OrdinalIgnoreCase))
                .Select(m => m.MachineType)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public async Task<IEnumerable<string>> GetMachinesWithLatestServicesAsync()
        {
            var mappings = await _repository.GetAllMappingAsync();
            var latestSeriesByAsset = mappings
                .GroupBy(m => m.AssetName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Max(m => ExtractSeriesNumber(m.SeriesNumber)),
                    StringComparer.OrdinalIgnoreCase
                );

            var machinesWithLatestSeries = mappings
                .GroupBy(machine => machine.MachineType, StringComparer.OrdinalIgnoreCase)
                .Where(machineGroup =>
                    machineGroup.All(mapping =>
                        ExtractSeriesNumber(mapping.SeriesNumber) == latestSeriesByAsset[mapping.AssetName]
                    )
                )
                .Select(g => g.Key)
                .ToList();

            return machinesWithLatestSeries;
        }
       
        public async Task<IEnumerable<string>> GetAllMachineTypesAsync()
        {
            return await _repository.GetAllMachineTypesAsync();
        }

        public async Task<IEnumerable<string>> GetAllAssetNamesAsync()
        {
            return await _repository.GetAllAssetNamesAsync();
        }
        
        public async Task<IEnumerable<MachineDetailDto>> GetAllMachineDetailsAsync()
        {
            var mappings = await _repository.GetAllMappingAsync();

            var latestSeriesByAsset = mappings
                .GroupBy(m => m.AssetName, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    g => g.Key,
                    g => g.Max(m => ExtractSeriesNumber(m.SeriesNumber)),
                    StringComparer.OrdinalIgnoreCase
                );

            var machineGroups = mappings.GroupBy(m => m.MachineType, StringComparer.OrdinalIgnoreCase);
            var result = new List<MachineDetailDto>();

            foreach (var group in machineGroups)
            {
                var assets = group.Select(m =>
                {
                    var seriesValue = ExtractSeriesNumber(m.SeriesNumber);
                    var isLatest = seriesValue == latestSeriesByAsset[m.AssetName];

                    return new AssetDetailDto
                    {
                        AssetName = m.AssetName,
                        SeriesNumber = m.SeriesNumber,
                        SeriesNumericValue = seriesValue,
                        IsLatestSeries = isLatest
                    };
                }).ToList();

                result.Add(new MachineDetailDto
                {
                    MachineType = group.Key,
                    Assets = assets,
                    UsesLatestSeries = assets.All(a => a.IsLatestSeries)
                });
            }

            return result.OrderBy(m => m.MachineType);
        }


        private int ExtractSeriesNumber(string seriesNumber)
        {
            if (string.IsNullOrEmpty(seriesNumber) || !seriesNumber.StartsWith("S"))
                throw new InvalidOperationException($"Invalid series number: {seriesNumber}");

            var numericPart = seriesNumber.Substring(1);
            if (int.TryParse(numericPart, out int value))
                return value;

            throw new InvalidOperationException($"Cannot parse series number: {seriesNumber}");
        }
    }
}
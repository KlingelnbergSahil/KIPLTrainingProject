
using KlingelnbergMachineManagement.Application.DTOs;

namespace KlingelnbergMachineManagement.Application.Services
{

    public interface IMachineService
    {
        Task<IEnumerable<AssetWithSeriesDto>> GetAssetsByMachineTypeAsync(string machineType);

        Task<IEnumerable<string>> GetMachinesByAssetNameAsync(string assetName);

        Task<IEnumerable<string>> GetMachinesWithLatestServicesAsync();

        Task<IEnumerable<string>> GetAllMachineTypesAsync();

        Task<IEnumerable<string>> GetAllAssetNamesAsync();

        Task<IEnumerable<MachineDetailDto>> GetAllMachineDetailsAsync();
    }
}

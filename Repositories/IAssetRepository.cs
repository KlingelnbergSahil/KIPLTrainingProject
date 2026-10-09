using KlingelnbergMachineManagement.Domain.Models;

namespace KlingelnbergMachineManagement.Domain.Interfaces
{

    public interface IAssetRepository
    {
        Task<IEnumerable<MachineAssetMapping>> GetAllMappingAsync();

        Task<IEnumerable<string>> GetAllMachineTypesAsync();

        Task<IEnumerable<string>> GetAllAssetNamesAsync();
    }
}

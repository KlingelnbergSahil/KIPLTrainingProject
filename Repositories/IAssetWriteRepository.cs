using KlingelnbergMachineManagement.Domain.Models;

namespace KlingelnbergMachineManagement.Domain.Interfaces 
{
    public interface IAssetWriteRepository
    {
        Task ReplaceAllAsync(IEnumerable<MachineAssetMapping> mappings);
        Task AppendAsync(IEnumerable<MachineAssetMapping> mappings);
    }
}

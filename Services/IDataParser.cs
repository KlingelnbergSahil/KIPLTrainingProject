using KlingelnbergMachineManagement.Domain.Models;

namespace KlingelnbergMachineManagement.Infrastructure.DataParsers
{
    public interface IDataParser
    {
        Task<IEnumerable<MachineAssetMapping>> ParseAsync(string filePath);

        bool CanParse(string filePath);
    }
}


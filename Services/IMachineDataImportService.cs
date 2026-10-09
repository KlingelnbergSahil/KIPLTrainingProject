namespace KlingelnbergMachineManagement.Application.Services
{
    public interface IMachineDataImportService
    {
        Task ImportAsync(Stream fileStream, string fileName, ImportMode mode);
    }

    public enum ImportMode
    {
        Replace,
        Append
    }
}
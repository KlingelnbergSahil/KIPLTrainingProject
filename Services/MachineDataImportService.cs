using KlingelnbergMachineManagement.Application.Services;
using KlingelnbergMachineManagement.Domain.Interfaces;
using KlingelnbergMachineManagement.Infrastructure.DataParsers;

public class MachineDataImportService : IMachineDataImportService
{
    private readonly IEnumerable<IDataParser> _parsers;
    private readonly IAssetWriteRepository _writeRepository;

    public MachineDataImportService(
        IEnumerable<IDataParser> parsers,
        IAssetWriteRepository writeRepository)
    {
        _parsers = parsers;
        _writeRepository = writeRepository;
    }

    public async Task ImportAsync(Stream fileStream, string fileName, ImportMode mode)
    {
        if (fileStream == null || fileStream.Length == 0)
            throw new ArgumentException("File is empty");

        var tempFilePath = Path.Combine(
            Path.GetTempPath(),
            $"{Guid.NewGuid()}_{fileName}");

        await using (var fs = new FileStream(tempFilePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(fs);
        }

        try
        {
            var parser = _parsers.FirstOrDefault(p => p.CanParse(tempFilePath))
                ?? throw new NotSupportedException("Unsupported file format");

            var mappings = (await parser.ParseAsync(tempFilePath)).ToList();

            if (!mappings.Any())
                throw new InvalidDataException("No valid data found");

            if (mode == ImportMode.Replace)
            {
                await _writeRepository.ReplaceAllAsync(mappings);
            }
            else
            {
                await _writeRepository.AppendAsync(mappings);
            }
        }
        finally
        {
            if (File.Exists(tempFilePath))
                File.Delete(tempFilePath);
        }
    }
}

using KlingelnbergMachineManagement.Domain.Interfaces;
using KlingelnbergMachineManagement.Domain.Models;
using KlingelnbergMachineManagement.Infrastructure.DataParsers;

public class AssetRepository : IAssetRepository, IDisposable
{
  private readonly IEnumerable<IDataParser> _parsers;
  private readonly string _dataFilePath;
  private IEnumerable<MachineAssetMapping>? _cachedMappings;
  private FileSystemWatcher? _fileWatcher;
  private readonly SemaphoreSlim _cacheLock = new(1, 1);

  public AssetRepository(IEnumerable<IDataParser> parsers, string dataFilePath) {
    _parsers = parsers ?? throw new ArgumentNullException(nameof(parsers));
    _dataFilePath = dataFilePath ?? throw new ArgumentNullException(nameof(dataFilePath));

    SetupFileWatcher();
  }

  private void SetupFileWatcher() {
    var directory = Path.GetDirectoryName(_dataFilePath);
    var fileName = Path.GetFileName(_dataFilePath);

    if (string.IsNullOrEmpty(directory) || string.IsNullOrEmpty(fileName))
      return;

    if (!Directory.Exists(directory))
      Directory.CreateDirectory(directory);

    _fileWatcher = new FileSystemWatcher(directory, fileName) {
      NotifyFilter = NotifyFilters.LastWrite
                     | NotifyFilters.Size
                     | NotifyFilters.FileName
    };

    _fileWatcher.Changed += (_, _) => InvalidateCache();
    _fileWatcher.Renamed += (_, _) => InvalidateCache();
    _fileWatcher.EnableRaisingEvents = true;
  }

  private void InvalidateCache() {
    _cachedMappings = null;
  }

  public async Task<IEnumerable<MachineAssetMapping>> GetAllMappingAsync() {
    if (_cachedMappings != null)
      return _cachedMappings;

    await _cacheLock.WaitAsync();
    try {
      if (_cachedMappings != null)
        return _cachedMappings;

      var parser = _parsers.FirstOrDefault(p => p.CanParse(_dataFilePath));
      if (parser == null)
        throw new NotSupportedException($"No parser for {_dataFilePath}");

      _cachedMappings = await parser.ParseAsync(_dataFilePath);
      return _cachedMappings;
    }
    finally {
      _cacheLock.Release();
    }
  }

  public async Task<IEnumerable<string>> GetAllMachineTypesAsync() {
    var mappings = await GetAllMappingAsync();
    return mappings
        .GroupBy(m => m.MachineType, StringComparer.OrdinalIgnoreCase)
        .Select(g => g.Key)
        .OrderBy(x => x);
  }

  public async Task<IEnumerable<string>> GetAllAssetNamesAsync() {
    var mappings = await GetAllMappingAsync();
    return mappings
        .GroupBy(m => m.AssetName, StringComparer.OrdinalIgnoreCase)
        .Select(g => g.Key)
        .OrderBy(x => x);
  }

  public void Dispose() {
    _fileWatcher?.Dispose();
    _cacheLock.Dispose();
  }
}

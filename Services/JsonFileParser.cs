using KlingelnbergMachineManagement.Domain.Models;
using System.Text.Json;

namespace KlingelnbergMachineManagement.Infrastructure.DataParsers
{

    public class JsonFileParser : IDataParser
    {
        private class JsonMapping
        {
            public string? MachineType { get; set; }
            public string? AssetName { get; set; }
            public string? SeriesNumber { get; set; }
        }

        public bool CanParse(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return false;

            return Path.GetExtension(filePath)
                .Equals(".json", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<IEnumerable<MachineAssetMapping>> ParseAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            try
            {
                var json = await File.ReadAllTextAsync(filePath);

                var mappings = JsonSerializer.Deserialize<List<JsonMapping>>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (mappings == null || mappings.Count == 0)
                    throw new InvalidDataException("JSON file contains no data");

                var result = new List<MachineAssetMapping>();

                for (int i = 0; i < mappings.Count; i++)
                {
                    var item = mappings[i];

                    try
                    {
                        if (string.IsNullOrWhiteSpace(item.MachineType) ||
                            string.IsNullOrWhiteSpace(item.AssetName) ||
                            string.IsNullOrWhiteSpace(item.SeriesNumber))
                        {
                            throw new InvalidDataException(
                                "MachineType, AssetName, and SeriesNumber are required");
                        }

                        result.Add(new MachineAssetMapping(
                            item.MachineType,
                            item.AssetName,
                            item.SeriesNumber
                        ));
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidDataException(
                            $"Error parsing JSON entry {i + 1}: {ex.Message}", ex);
                    }
                }

                return result;
            }
            catch (JsonException ex)
            {
                throw new InvalidDataException(
                    $"Invalid JSON format: {ex.Message}", ex);
            }
        }
    }
}

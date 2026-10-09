using KlingelnbergMachineManagement.Domain.Models;
using KlingelnbergMachineManagement.Infrastructure.DataParsers;
using System.Text.RegularExpressions;

namespace KlingelnbergMachineManagement.Services
{
    public class TextFileParser :IDataParser
    {
        private static readonly Regex SeriesPattern = new Regex(@"^S\d+$", RegexOptions.Compiled);


        public bool CanParse(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            var extension = Path.GetExtension(filePath);
            return extension == ".txt" || extension == ".csv";
        }


        public async Task<IEnumerable<MachineAssetMapping>> ParseAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not foundddd {filePath}");

            var mappings = new List<MachineAssetMapping>();
            var lines = await File.ReadAllLinesAsync(filePath);

            for(int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                try
                {
                    var mapping = ParseLine(line);
                    mappings.Add(mapping);
                } catch(Exception ex)
                {
                    throw new InvalidDataException($"Error parsing line {i + 1}: {line}. {ex.Message}", ex);
                }
            }
            return mappings;
        }

        public MachineAssetMapping ParseLine(string line)
        {
            var parts = line.Split(',', StringSplitOptions.TrimEntries);
            if (parts.Length != 3)
                throw new FormatException($"Expected 3 comma-seprated values, but found {parts.Length}");

            var machineType = parts[0];
            var assetName = parts[1];
            var seriesNumber = parts[2];

            if (!SeriesPattern.IsMatch(seriesNumber))
                throw new FormatException($"Invalid series number format : {seriesNumber}. Expected format: S<digits>");

            return new MachineAssetMapping(machineType, assetName, seriesNumber);
        }
    }
}

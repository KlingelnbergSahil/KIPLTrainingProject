namespace KlingelnbergMachineManagement.Application.DTOs
{

    public class MachineDetailDto
    {
        public string MachineType { get; set; } = string.Empty;
        public List<AssetDetailDto> Assets { get; set; } = new();
        public bool UsesLatestSeries {  get; set; }
    }

    public class AssetDetailDto 
    { 
        public string AssetName {  get; set; }= string.Empty;
        public string SeriesNumber { get; set; } = string.Empty;
        public int SeriesNumericValue { get; set; }
        public bool IsLatestSeries { get; set; }
    }

    public class AssetSeriesInfo
    {
        public string AssetName { get; set; }= string.Empty;
        public string SeriesNumber { get; set; } = string.Empty;
        public int SeriesValue { get; set; }
    }

    public class AssetWithSeriesDto
    {
        public string AssetName { get; set; } = string.Empty;
        public string SeriesNumber { get; set; } = string.Empty;
    }
}
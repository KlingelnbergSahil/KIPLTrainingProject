using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace KlingelnbergMachineManagement.Domain.Models
{
    
    public class MachineAssetMapping
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string MachineType {  get; set; }
        public string AssetName { get; set; }
        public string SeriesNumber { get; set; }

        public MachineAssetMapping(string machineType, string assetName, string seriesNumber)
        {
            if(string.IsNullOrWhiteSpace(machineType))
                throw new ArgumentException("Machine type cannot be empty", nameof(machineType));
            if (string.IsNullOrWhiteSpace(assetName))
                throw new ArgumentException("Machine type cannot be empty", nameof(assetName));
            if (string.IsNullOrWhiteSpace(seriesNumber))
                throw new ArgumentException("Machine type cannot be empty", nameof(seriesNumber));

            MachineType = machineType;
            AssetName = assetName;
            SeriesNumber = seriesNumber;
        }

        public override string ToString()
        {
            return $"{MachineType} -> {AssetName} ({SeriesNumber})";
        }
    }
}
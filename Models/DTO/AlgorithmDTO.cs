using JsonSubTypes;
using Newtonsoft.Json;

namespace MerryClosets.Models.DTO
{
    [JsonConverter(typeof(JsonSubtypes), "$type")]
    [JsonSubtypes.KnownSubType(typeof(MaterialFinishPartAlgorithmDto), "materialFinishPartAlgorithm")]
    [JsonSubtypes.KnownSubType(typeof(MaterialPartAlgorithmDto), "materialPartAlgorithm")]
    [JsonSubtypes.KnownSubType(typeof(RatioAlgorithmDto), "ratioAlgorithm")]
    [JsonSubtypes.KnownSubType(typeof(SizePartAlgorithmDto), "sizePartAlgorithm")]
    [JsonSubtypes.KnownSubType(typeof(SizePercentagePartAlgorithmDto), "sizePercentagePartAlgorithm")]
    public class AlgorithmDto : ValueObjectDto
    {
        public enum RestrictionDtoType
        {
            MaterialFinishPartAlgorithmDto = 1,
            MaterialPartAlgorithmDto = 2,
            RatioAlgorithmDto = 3,
            SizePartAlgorithmDto = 4,
            SizePercentagePartAlgorithmDto = 5
        }
    }
}
using JsonSubTypes;
using MerryClosets.Models.Restriction;
using Newtonsoft.Json;

namespace MerryClosets.Models.DTO
{
    [JsonConverter(typeof(JsonSubtypes), "$type")]
    [JsonSubtypes.KnownSubType(typeof(SizePercentagePartAlgorithmDto), "sizePercentagePartAlgorithm")]
    [JsonSubtypes.KnownSubType(typeof(MaterialFinishPartAlgorithmDto), "materialFinishPartAlgorithm")]
    public class PartAlgorithmDto : AlgorithmDto
    {
        public enum RestrictionDtoType
        {
            SizePercentagePartAlgorithmDto = 1,
            MaterialFinishPartAlgorithmDto = 2
        }
    }
}
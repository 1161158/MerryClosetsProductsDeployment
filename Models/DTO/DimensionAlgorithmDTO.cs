using JsonSubTypes;
using Newtonsoft.Json;

namespace MerryClosets.Models.DTO
{
    [JsonConverter(typeof(JsonSubtypes), "$type")]
    [JsonSubtypes.KnownSubType(typeof(RatioAlgorithmDto), "ratioAlgorithm")]
    public class DimensionAlgorithmDto : AlgorithmDto
    {
        public enum RestrictionDtoType
        {
            RatioAlgorithmDto = 1
        }
    }
}
using JsonSubTypes;
using Newtonsoft.Json;
using MerryClosets.Utils;

namespace MerryClosets.Models.DTO
{
    [JsonConverter(typeof(JsonSubtypes), "type")]
    [JsonSubtypes.KnownSubType(typeof(RatioAlgorithmDto), Strings.RATIO_ALGORITHM)]
    public abstract class DimensionAlgorithmDto : AlgorithmDto
    {
        public enum RestrictionDtoType
        {
            RatioAlgorithmDto = 1
        }
    }
}
using JsonSubTypes;
using Newtonsoft.Json;
using MerryClosets.Utils;

namespace MerryClosets.Models.DTO
{
    [JsonConverter(typeof(JsonSubtypes), "type")]
    [JsonSubtypes.KnownSubType(typeof(MaterialFinishPartAlgorithmDto), Strings.MATERIAL_FINISH_PART_ALGORITHM)]
    [JsonSubtypes.KnownSubType(typeof(MaterialPartAlgorithmDto), Strings.MATERIAL_PART_ALGORITHM)]
    [JsonSubtypes.KnownSubType(typeof(RatioAlgorithmDto), Strings.RATIO_ALGORITHM)]
    [JsonSubtypes.KnownSubType(typeof(SizePartAlgorithmDto), Strings.SIZE_PART_ALGORITHM)]
    [JsonSubtypes.KnownSubType(typeof(SizePercentagePartAlgorithmDto), Strings.SIZE_PERCENTAGE_PART_ALGORITHM)]
    public abstract class AlgorithmDto : ValueObjectDto
    {
        public string type;
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
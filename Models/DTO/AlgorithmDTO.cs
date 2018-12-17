using JsonSubTypes;
using Newtonsoft.Json;
using MerryClosets.Utils;

namespace MerryClosets.Models.DTO
{
    [JsonConverter(typeof(JsonSubtypes), "type")]
    [JsonSubtypes.KnownSubType(typeof(MaterialFinishPartAlgorithmDto), RestrictionName.MATERIAL_FINISH_PART_ALGORITHM)]
    [JsonSubtypes.KnownSubType(typeof(MaterialPartAlgorithmDto), RestrictionName.MATERIAL_PART_ALGORITHM)]
    [JsonSubtypes.KnownSubType(typeof(RatioAlgorithmDto), RestrictionName.RATIO_ALGORITHM)]
    [JsonSubtypes.KnownSubType(typeof(SizePartAlgorithmDto), RestrictionName.SIZE_PART_ALGORITHM)]
    [JsonSubtypes.KnownSubType(typeof(SizePercentagePartAlgorithmDto), RestrictionName.SIZE_PERCENTAGE_PART_ALGORITHM)]
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
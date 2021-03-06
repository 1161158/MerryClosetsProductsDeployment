using JsonSubTypes;
using MerryClosets.Models.Restriction;
using Newtonsoft.Json;
using MerryClosets.Utils;

namespace MerryClosets.Models.DTO
{
    [JsonConverter(typeof(JsonSubtypes), "type")]
    [JsonSubtypes.KnownSubType(typeof(SizePercentagePartAlgorithmDto), Strings.SIZE_PERCENTAGE_PART_ALGORITHM)]
    [JsonSubtypes.KnownSubType(typeof(MaterialFinishPartAlgorithmDto), Strings.MATERIAL_FINISH_PART_ALGORITHM)]
    public abstract class PartAlgorithmDto : AlgorithmDto
    {
        public enum RestrictionDtoType
        {
            SizePercentagePartAlgorithmDto = 1,
            MaterialFinishPartAlgorithmDto = 2
        }
    }
}
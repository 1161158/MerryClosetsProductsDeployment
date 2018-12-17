using System.Collections.Generic;
using JsonSubTypes;
using MerryClosets.Models.DTO;
using Newtonsoft.Json;
using MerryClosets.Utils;

namespace MerryClosets.Models.Restriction
{
    [JsonConverter(typeof(JsonSubtypes), "$type")]
    [JsonSubtypes.KnownSubType(typeof(MaterialFinishPartAlgorithm), RestrictionName.MATERIAL_FINISH_PART_ALGORITHM)]
    [JsonSubtypes.KnownSubType(typeof(MaterialPartAlgorithm), RestrictionName.MATERIAL_PART_ALGORITHM)]
    [JsonSubtypes.KnownSubType(typeof(RatioAlgorithm), RestrictionName.RATIO_ALGORITHM)]
    [JsonSubtypes.KnownSubType(typeof(SizePartAlgorithm), RestrictionName.SIZE_PART_ALGORITHM)]
    [JsonSubtypes.KnownSubType(typeof(SizePercentagePartAlgorithm), RestrictionName.SIZE_PERCENTAGE_PART_ALGORITHM)]
    public abstract class Algorithm : ValueObject
    {
        public abstract bool validate(Dictionary<string, object> parameters);

        public abstract Dictionary<string, string> parameters();

        public enum RestrictionType
        {
            MaterialFinishPartAlgorithm = 1,
            MaterialPartAlgorithm = 2,
            RatioAlgorithm = 3,
            SizePartAlgorithm = 4,
            SizePercentagePartAlgorithm = 5
        }

    }
}
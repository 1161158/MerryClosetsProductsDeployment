using System.Collections.Generic;
using JsonSubTypes;
using MerryClosets.Models.DTO;
using Newtonsoft.Json;

namespace MerryClosets.Models.Restriction
{
    [JsonConverter(typeof(JsonSubtypes), "$type")]
    [JsonSubtypes.KnownSubType(typeof(MaterialFinishPartAlgorithm), "materialFinishPartAlgorithm")]
    [JsonSubtypes.KnownSubType(typeof(MaterialPartAlgorithm), "materialPartAlgorithm")]
    [JsonSubtypes.KnownSubType(typeof(RatioAlgorithm), "ratioAlgorithm")]
    [JsonSubtypes.KnownSubType(typeof(SizePartAlgorithm), "sizePartAlgorithm")]
    [JsonSubtypes.KnownSubType(typeof(SizePercentagePartAlgorithm), "sizePercentagePartAlgorithm")]
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
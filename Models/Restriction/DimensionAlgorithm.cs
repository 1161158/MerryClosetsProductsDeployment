using System.Collections.Generic;
using JsonSubTypes;
using Newtonsoft.Json;
using MerryClosets.Utils;

namespace MerryClosets.Models.Restriction
{
    [JsonConverter(typeof(JsonSubtypes), "$type")]
    [JsonSubtypes.KnownSubType(typeof(RatioAlgorithm), RestrictionName.RATIO_ALGORITHM)]
    public abstract class DimensionAlgorithm : Algorithm
    {
        //public abstract bool validate(ConfiguredDimension inputedDimensions);
        public static readonly List<string> AvailableDimensionAlgorithms =
            new List<string>(new string[] {RestrictionName.RATIO_ALGORITHM});

        public enum RestrictionType
        {
            RatioAlgorithm = 1
        }
    }
}
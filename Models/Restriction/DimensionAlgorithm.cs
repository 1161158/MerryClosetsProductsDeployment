using System.Collections.Generic;
using JsonSubTypes;
using Newtonsoft.Json;

namespace MerryClosets.Models.Restriction
{
    [JsonConverter(typeof(JsonSubtypes), "$type")]
    [JsonSubtypes.KnownSubType(typeof(RatioAlgorithm), "RatioAlgorithm")]
    public abstract class DimensionAlgorithm : Algorithm
    {
        //public abstract bool validate(ConfiguredDimension inputedDimensions);
        public static readonly List<string> AvailableDimensionAlgorithms =
            new List<string>(new string[] {"RatioAlgorithm"});

        public enum RestrictionType
        {
            RatioAlgorithm = 1
        }
    }
}
using System.Collections.Generic;
using JsonSubTypes;
using Newtonsoft.Json;

namespace MerryClosets.Models.Restriction
{
    [JsonConverter(typeof(JsonSubtypes), "$type")]
    [JsonSubtypes.KnownSubType(typeof(SizePercentagePartAlgorithm), "SizePercentagePartAlgorithm")]
    [JsonSubtypes.KnownSubType(typeof(MaterialFinishPartAlgorithm), "MaterialFinishPartAlgorithm")]
    public abstract class PartAlgorithm : Algorithm
    {
        //public abstract bool validate(Product.Product partProduct, List<ConfiguredProduct.ConfiguredProduct> configuredProductChildren);
        public static readonly List<string> AvailablePartAlgorithms = new List<string>(new string[]
        {
            "MaterialPartAlgorithm",
            "SizePartAlgorithm",
            "MaterialFinishPartAlgorithm",
            "SizePercentagePartAlgorithm"
        });

        public enum RestrictionType
        {
            SizePercentagePartAlgorithm = 1,
            MaterialFinishPartAlgorithm = 2
        }
    }
}
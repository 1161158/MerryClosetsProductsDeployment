using System.Collections.Generic;
using JsonSubTypes;
using Newtonsoft.Json;
using MerryClosets.Utils;

namespace MerryClosets.Models.Restriction
{
    [JsonConverter(typeof(JsonSubtypes), "type")]
    [JsonSubtypes.KnownSubType(typeof(SizePercentagePartAlgorithm), Strings.SIZE_PERCENTAGE_PART_ALGORITHM)]
    [JsonSubtypes.KnownSubType(typeof(MaterialFinishPartAlgorithm), Strings.MATERIAL_FINISH_PART_ALGORITHM)]
    public abstract class PartAlgorithm : Algorithm
    {
        //public abstract bool validate(Product.Product partProduct, List<ConfiguredProduct.ConfiguredProduct> configuredProductChildren);
        public static readonly List<string> AvailablePartAlgorithms = new List<string>(new string[]
        {
            "MaterialPartAlgorithm",
            "SizePartAlgorithm",
            Strings.MATERIAL_FINISH_PART_ALGORITHM,
            Strings.SIZE_PERCENTAGE_PART_ALGORITHM
        });

        public enum RestrictionType
        {
            SizePercentagePartAlgorithm = 1,
            MaterialFinishPartAlgorithm = 2
        }
    }
}
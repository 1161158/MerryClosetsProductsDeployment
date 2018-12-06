using System.Collections.Generic;

namespace MerryClosets.Models.Restriction
{
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
    }
}
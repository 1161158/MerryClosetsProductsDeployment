using System.Collections.Generic;
using MerryClosets.Models.ConfiguredProduct;
using MerryClosets.Models.Product;

namespace MerryClosets.Models.Restriction
{

    public abstract class DimensionAlgorithm : Algorithm
    {
        //public abstract bool validate(ConfiguredDimension inputedDimensions);
        public static readonly List<string> AvailableDimensionAlgorithms = new List<string>(new string[] { "RatioAlgorithm" });
    }
}
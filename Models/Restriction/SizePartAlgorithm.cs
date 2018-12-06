using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MerryClosets.Models.ConfiguredProduct;
using MerryClosets.Models.Product;

namespace MerryClosets.Models.Restriction
{
    //Not used
    public class SizePartAlgorithm : PartAlgorithm
    {
        
        private string Part { get; set; }
        private string Configured { get; set; }

        public SizePartAlgorithm(string first, string second)
        {
            this.Part = second;
            this.Configured = first;
        }

        protected SizePartAlgorithm(){}

        // private bool Validate(List<Values> list, float dimension)
        // {
        //     foreach (var value in list)
        //     {
        //         var listValues = value.ObtainValues();
        //         if (listValues.Count > 1) // significa que é continuous
        //         {
        //             if (dimension - listValues[0] >= 0.0f && dimension - listValues[1] <= 0.0f)
        //             {
        //                 return true;
        //             }
        //         } 
        //         else if (listValues.Count == 1) // significa que é discrete
        //         {
        //             if (dimension - listValues[0] == 0.0f) 
        //             {
        //                 return true;
        //             }
        //         }
        //     }
        //     return false;
        // }

        public override bool validate(Dictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        public override Dictionary<string, string> parameters()
        {
            throw new NotImplementedException();
        }
        /*
public override bool validate(Dictionary<string, object> parameters)
{
var configured = (ConfiguredProduct.ConfiguredProduct)parameters[Configured];
var part = (Product.Part) parameters[Part];
var listPartSizes = part.Product.Sizes;
var configuredSize = configured.Dimension;

foreach (var values in listPartSizes)
{
bool validateHeight = Validate(values.Height, configuredSize.Height);
bool validateWidth = Validate(values.Width, configuredSize.Width);
bool validateDepth = Validate(values.Depth, configuredSize.Depth);

if (validateDepth && validateHeight && validateWidth)
{
  return true;
}
}
return false;
}

*/
    }
}
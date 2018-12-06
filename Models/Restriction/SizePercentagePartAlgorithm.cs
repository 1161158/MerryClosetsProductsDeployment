using System;
using System.Collections.Generic;
using MerryClosets.Models.DTO;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Infrastructure;

namespace MerryClosets.Models.Restriction
{
    public class SizePercentagePartAlgorithm : PartAlgorithm
    {
        public string SizeType { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }

        public SizePercentagePartAlgorithm(string sizeType, int min, int max)
        {
            this.SizeType = sizeType;
            this.Min = min;
            this.Max = max;
        }

        protected SizePercentagePartAlgorithm() { }

        public override Dictionary<string, string> parameters()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("SizeType", "Type of size (height, depth or width)");
            parameters.Add("Min", "The minimum boundry for the calculation as an integer percentage");
            parameters.Add("Max", "The maximum boundry for the calculation as an integer percentage");
            return parameters;
        }

        public override bool validate(Dictionary<string, object> parameters)
        {
            var parent = (ConfiguredProduct.ConfiguredProduct)parameters["ParentConfiguredProduct"];
            var child = (ConfiguredProductDto)parameters["ChildConfiguredProduct"];

            float relation = relationCalculation(child, parent) * 100;

            return relation <= Max && relation >= Min;
        }

        private float relationCalculation(ConfiguredProductDto child, ConfiguredProduct.ConfiguredProduct parent)
        {
            float firstValue;
            float secondValue;
            switch (SizeType)
            {
                case "height":
                    firstValue = (int)child.ConfiguredDimension.Height;
                    secondValue = (int)parent.ConfiguredDimension.Height;
                    return firstValue / secondValue;

                case "width":
                    firstValue = (int)child.ConfiguredDimension.Width;
                    secondValue = (int)parent.ConfiguredDimension.Width;
                    return firstValue / secondValue;

                case "depth":
                    firstValue = (int)child.ConfiguredDimension.Depth;
                    secondValue = (int)parent.ConfiguredDimension.Depth;
                    return firstValue / secondValue;
            }
            return -1;
        }

        public override bool Equals(object obj)
        {
            
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            SizePercentagePartAlgorithm other = (SizePercentagePartAlgorithm) obj;
            return string.Equals(this.SizeType, other.SizeType, StringComparison.Ordinal);
        }
        
        public override int GetHashCode()
        {
            return System.Tuple.Create(this.SizeType).GetHashCode();
        }
    }
}
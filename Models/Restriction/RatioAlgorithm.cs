using System;
using System.Collections.Generic;
using MerryClosets.Models.ConfiguredProduct;
using MerryClosets.Models.DTO;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace MerryClosets.Models.Restriction
{
    public class RatioAlgorithm : DimensionAlgorithm
    {
        public string FirstValueDesc { get; set; }
        public string SecondValueDesc { get; set; }
        public string Operator { get; set; }
        public float Ratio { get; set; }

        public RatioAlgorithm(string firstValueDesc, string secondValueDesc, string op, float ratio)
        {
            this.FirstValueDesc = firstValueDesc;
            this.SecondValueDesc = secondValueDesc;
            this.Operator = op;
            this.Ratio = ratio;
        }

        protected RatioAlgorithm() { }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            RatioAlgorithm other = (RatioAlgorithm)obj;
            return string.Equals(this.FirstValueDesc, other.FirstValueDesc, StringComparison.Ordinal)
             && string.Equals(this.SecondValueDesc, other.SecondValueDesc, StringComparison.Ordinal)
             && string.Equals(this.Operator,other.Operator, StringComparison.Ordinal)
             && this.Ratio == other.Ratio;
        }

        public override int GetHashCode()
        {
            return System.Tuple.Create(this.FirstValueDesc, this.SecondValueDesc, this.Operator, this.Ratio).GetHashCode();;
        }

        public override bool validate(Dictionary<string, object> parameters)
        {
            float firstValue = (int)parameters[FirstValueDesc];
            float secondValueDesc = (int)parameters[SecondValueDesc];
            float calculatedRatio = firstValue / secondValueDesc;
            switch (Operator)
            {
                case "<":
                    return calculatedRatio < Ratio;
                case ">":
                    return calculatedRatio > Ratio;
                case ">=":
                    return calculatedRatio >= Ratio;
                case "<=":
                    return calculatedRatio >= Ratio;
                default:
                    return false;
            }
        }

        public override Dictionary<string, string> parameters()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("First Value", "The first value to be added");
            parameters.Add("Second Value", "The second value to be added");
            parameters.Add("Operator", "One of these: <, >, <=, >=");
            parameters.Add("Ratio", "Value for the ration between the first and second values");
            return parameters;
        }
    }
}
using System.Collections.Generic;
using Newtonsoft.Json;
using MerryClosets.Utils;

namespace MerryClosets.Models.DTO
{
    public class ContinuousValueDto : ValuesDto
    {
        public int MinValue { get; set; }
        public int MaxValue { get; set; }


        public ContinuousValueDto(int minValue, int maxValue)
        {
            this.MinValue = minValue;
            this.MaxValue = maxValue;
            base.type = Strings.CONTINUOUS_VALUE;
        }

        protected ContinuousValueDto() { }
        
        public override string ToString()
        {
            return base.ToString() + "min= " + this.MinValue.ToString() + " max= " + this.MaxValue.ToString();
        }
    }
}
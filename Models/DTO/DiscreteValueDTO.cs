using System.Collections.Generic;
using Newtonsoft.Json;
using MerryClosets.Utils;

namespace MerryClosets.Models.DTO
{
    public class DiscreteValueDto : ValuesDto
    {
        public int Value { get; set; }

        public DiscreteValueDto(int value)
        {
            this.Value = value;
            base.type = Strings.DISCRETE_VALUE;
        }

        protected DiscreteValueDto() { }
        public override string ToString()
        {
            return base.ToString() + this.Value.ToString();
        }
    }
}
using System.Collections.Generic;
using JsonSubTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MerryClosets.Utils;

namespace MerryClosets.Models.DTO
{
    [JsonConverter(typeof(JsonSubtypes), "type")]
    [JsonSubtypes.KnownSubType(typeof(DiscreteValueDto), Strings.DISCRETE_VALUE)]
    [JsonSubtypes.KnownSubType(typeof(ContinuousValueDto), Strings.CONTINUOUS_VALUE)]
    public abstract class ValuesDto : ValueObjectDto
    {
        public string type;
        public enum ValuesDtoType
        {
            DiscreteValueDto = 1,
            ContinuousValueDto = 2
        }

        public override string ToString()
        {
            return "";
        }
    }
}
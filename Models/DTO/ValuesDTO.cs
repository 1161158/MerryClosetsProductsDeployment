using System.Collections.Generic;
using JsonSubTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MerryClosets.Models.DTO
{
    [JsonConverter(typeof(JsonSubtypes), "$type")]
    [JsonSubtypes.KnownSubType(typeof(DiscreteValueDto), "discrete")]
    [JsonSubtypes.KnownSubType(typeof(ContinuousValueDto), "continuous")]
    public abstract class ValuesDto : ValueObjectDto
    {

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
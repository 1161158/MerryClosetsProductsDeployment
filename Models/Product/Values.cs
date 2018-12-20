using JsonSubTypes;
using Newtonsoft.Json;
using MerryClosets.Utils;

namespace MerryClosets.Models.Product
{
    [JsonConverter(typeof(JsonSubtypes), "type")]
    [JsonSubtypes.KnownSubType(typeof(DiscreteValue), Strings.DISCRETE_VALUE)]
    [JsonSubtypes.KnownSubType(typeof(ContinuousValue), Strings.CONTINUOUS_VALUE)]
    public abstract class Values : ValueObject
    {
        public abstract bool IsValid(int value);

        public enum ValuesType
        {
            DiscreteValue = 1,
            ContinuousValue = 2
        }
    }
}
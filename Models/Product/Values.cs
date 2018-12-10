using JsonSubTypes;
using Newtonsoft.Json;

namespace MerryClosets.Models.Product
{
    [JsonConverter(typeof(JsonSubtypes))]
    [JsonSubtypes.KnownSubType(typeof(DiscreteValue), "discrete")]
    [JsonSubtypes.KnownSubType(typeof(ContinuousValue), "continuous")]
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
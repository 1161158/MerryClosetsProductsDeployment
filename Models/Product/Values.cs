using JsonSubTypes;
using Newtonsoft.Json;
using MerryClosets.Utils;

namespace MerryClosets.Models.Product
{
    [JsonConverter(typeof(JsonSubtypes))]
    [JsonSubtypes.KnownSubType(typeof(DiscreteValue), RestrictionName.DISCRETE_VALUE)]
    [JsonSubtypes.KnownSubType(typeof(ContinuousValue), RestrictionName.CONTINUOUS_VALUE)]
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
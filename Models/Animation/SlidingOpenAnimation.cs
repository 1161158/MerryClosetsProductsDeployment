using JsonSubTypes;
using Newtonsoft.Json;

namespace MerryClosets.Models.Animation
{
    [JsonConverter(typeof(JsonSubtypes), "type")]
    [JsonSubtypes.KnownSubType(typeof(SlidingLeftAnimation), "slidingLeftAnimation")]
    [JsonSubtypes.KnownSubType(typeof(SlidingRightAnimation), "slidingRightAnimation")]
    public abstract class SlidingOpenAnimation : Animation
    {
        public enum SlidingOpenAnimationType
        {
            SlidingLeftAnimation = 1,
            SlidingRightAnimation = 2
        }
    }
}
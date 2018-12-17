using System.Collections.Generic;
using JsonSubTypes;
using Newtonsoft.Json;

namespace MerryClosets.Models.Animation
{
    [JsonConverter(typeof(JsonSubtypes), "type")]
    [JsonSubtypes.KnownSubType(typeof(LateralOpenAnimation), "lateralOpenAnimation")]
    [JsonSubtypes.KnownSubType(typeof(FrontalOpenAnimation), "frontalOpenAnimation")]
    [JsonSubtypes.KnownSubType(typeof(SlidingLeftAnimation), "slidingLeftAnimation")]
    [JsonSubtypes.KnownSubType(typeof(SlidingRightAnimation), "slidingRightAnimation")]
    public abstract class Animation : ValueObject
    {
        public string type { get; set; }
        public enum AnimationType
        {
            LateralOpenAnimation = 1,
            FrontalOpenAnimation = 2,
            SlidingLeftAnimation = 3,
            SlidingRightAnimation = 4
        }
    }
}
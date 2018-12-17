using JsonSubTypes;
using Newtonsoft.Json;

namespace MerryClosets.Models.DTO
{
    [JsonConverter(typeof(JsonSubtypes), "type")]
    [JsonSubtypes.KnownSubType(typeof(SlidingLeftAnimationDto), "slidingLeftAnimation")]
    [JsonSubtypes.KnownSubType(typeof(SlidingRightAnimationDto), "slidingRightAnimation")]
    public abstract class SlidingOpenAnimationDto : AnimationDto
    {
        public enum SlidingOpenAnimationDtoType
        {
            SlidingLeftAnimationDto = 1,
            SlidingRightAnimationDto = 2
        }
    }
}
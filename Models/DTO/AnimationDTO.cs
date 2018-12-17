using JsonSubTypes;
using Newtonsoft.Json;

namespace MerryClosets.Models.DTO
{
    [JsonConverter(typeof(JsonSubtypes), "type")]
    [JsonSubtypes.KnownSubType(typeof(LateralOpenAnimationDto), "lateralOpenAnimation")]
    [JsonSubtypes.KnownSubType(typeof(FrontalOpenAnimationDto), "frontalOpenAnimation")]
    [JsonSubtypes.KnownSubType(typeof(SlidingLeftAnimationDto), "slidingLeftAnimation")]
    [JsonSubtypes.KnownSubType(typeof(SlidingRightAnimationDto), "slidingRightAnimation")]
    public class AnimationDto : ValueObjectDto
    {
        public string type { get; set; }
        public enum AnimationDtoType
        {
            LateralOpenAnimationDto = 1,
            FrontalOpenAnimationDto = 2,
            SlidingLeftAnimationDto = 3,
            SlidingRightAnimationDto = 4
        }
    }
}
using JsonSubTypes;
using Newtonsoft.Json;
using MerryClosets.Models.DTO;
using MerryClosets.Models.Product;
using MerryClosets.Models.Restriction;
using MerryClosets.Models.Animation;

namespace MerryClosets.Configurations
{
    public class AbstractClassConfiguration{
        public static void configure()
        {
            useDefault();
        }

        private static void useDefault()
        {
            var settings = new JsonSerializerSettings();
            //Values 
            settings.Converters.Add(JsonSubtypesConverterBuilder.Of(typeof(ValuesDto), "type")
                .RegisterSubtype(typeof(DiscreteValueDto), ValuesDto.ValuesDtoType.DiscreteValueDto)
                .RegisterSubtype(typeof(ContinuousValueDto), ValuesDto.ValuesDtoType.ContinuousValueDto)
                .SerializeDiscriminatorProperty().Build());
            settings.Converters.Add(JsonSubtypesConverterBuilder.Of(typeof(Values), "type")
                .RegisterSubtype(typeof(DiscreteValue), Values.ValuesType.DiscreteValue)
                .RegisterSubtype(typeof(ContinuousValue), Values.ValuesType.ContinuousValue)
                .SerializeDiscriminatorProperty().Build());
            
            //Algorithm
            settings.Converters.Add(JsonSubtypesConverterBuilder.Of(typeof(AlgorithmDto), "type")
                .RegisterSubtype(typeof(MaterialFinishPartAlgorithmDto), AlgorithmDto.RestrictionDtoType.MaterialFinishPartAlgorithmDto)
                .RegisterSubtype(typeof(RatioAlgorithmDto), AlgorithmDto.RestrictionDtoType.RatioAlgorithmDto)
                .RegisterSubtype(typeof(SizePercentagePartAlgorithmDto), AlgorithmDto.RestrictionDtoType.SizePercentagePartAlgorithmDto)
                .SerializeDiscriminatorProperty().Build());
            settings.Converters.Add(JsonSubtypesConverterBuilder.Of(typeof(Algorithm), "type")
                .RegisterSubtype(typeof(MaterialFinishPartAlgorithm), Algorithm.RestrictionType.MaterialFinishPartAlgorithm)
                .RegisterSubtype(typeof(RatioAlgorithm), Algorithm.RestrictionType.RatioAlgorithm)
                .RegisterSubtype(typeof(SizePercentagePartAlgorithm), Algorithm.RestrictionType.SizePercentagePartAlgorithm)
                .SerializeDiscriminatorProperty().Build());
            
            //DimensionAlgorithm
            settings.Converters.Add(JsonSubtypesConverterBuilder.Of(typeof(DimensionAlgorithmDto), "type")
                .RegisterSubtype(typeof(RatioAlgorithmDto), DimensionAlgorithmDto.RestrictionDtoType.RatioAlgorithmDto)
                .SerializeDiscriminatorProperty().Build());
            settings.Converters.Add(JsonSubtypesConverterBuilder.Of(typeof(DimensionAlgorithm), "type")
                .RegisterSubtype(typeof(RatioAlgorithm), DimensionAlgorithm.RestrictionType.RatioAlgorithm)
                .SerializeDiscriminatorProperty().Build());
            
            //PartAlgorithm - SizePercentagePartAlgorithm
            settings.Converters.Add(JsonSubtypesConverterBuilder.Of(typeof(PartAlgorithmDto), "type")
                .RegisterSubtype(typeof(SizePercentagePartAlgorithmDto), PartAlgorithmDto.RestrictionDtoType.SizePercentagePartAlgorithmDto)
                .SerializeDiscriminatorProperty().Build());
            settings.Converters.Add(JsonSubtypesConverterBuilder.Of(typeof(PartAlgorithm), "type")
                .RegisterSubtype(typeof(SizePercentagePartAlgorithm), PartAlgorithm.RestrictionType.SizePercentagePartAlgorithm)
                .SerializeDiscriminatorProperty().Build());
            
            //PartAlgorithm - MaterialFinishPartAlgorithm
            settings.Converters.Add(JsonSubtypesConverterBuilder.Of(typeof(PartAlgorithmDto), "type")
                .RegisterSubtype(typeof(MaterialFinishPartAlgorithmDto), PartAlgorithmDto.RestrictionDtoType.MaterialFinishPartAlgorithmDto)
                .SerializeDiscriminatorProperty().Build());
            settings.Converters.Add(JsonSubtypesConverterBuilder.Of(typeof(PartAlgorithm), "type")
                .RegisterSubtype(typeof(MaterialFinishPartAlgorithm), PartAlgorithm.RestrictionType.MaterialFinishPartAlgorithm)
                .SerializeDiscriminatorProperty().Build());

            //Animation
            settings.Converters.Add(JsonSubtypesConverterBuilder.Of(typeof(AnimationDto), "type")
                .RegisterSubtype(typeof(FrontalOpenAnimationDto), AnimationDto.AnimationDtoType.FrontalOpenAnimationDto)
                .RegisterSubtype(typeof(LateralOpenAnimationDto), AnimationDto.AnimationDtoType.LateralOpenAnimationDto)
                .RegisterSubtype(typeof(SlidingLeftAnimationDto), AnimationDto.AnimationDtoType.SlidingLeftAnimationDto)
                .RegisterSubtype(typeof(SlidingRightAnimationDto), AnimationDto.AnimationDtoType.SlidingRightAnimationDto)
                .SerializeDiscriminatorProperty().Build());
            settings.Converters.Add(JsonSubtypesConverterBuilder.Of(typeof(Animation), "type")
                .RegisterSubtype(typeof(FrontalOpenAnimation), Animation.AnimationType.FrontalOpenAnimation)
                .RegisterSubtype(typeof(LateralOpenAnimation), Animation.AnimationType.LateralOpenAnimation)
                .RegisterSubtype(typeof(SlidingLeftAnimation), Animation.AnimationType.SlidingLeftAnimation)
                .RegisterSubtype(typeof(SlidingRightAnimation), Animation.AnimationType.SlidingRightAnimation)
                .SerializeDiscriminatorProperty().Build());

            //SlidingOpenAnimation
            settings.Converters.Add(JsonSubtypesConverterBuilder.Of(typeof(SlidingOpenAnimationDto), "type")
                .RegisterSubtype(typeof(SlidingLeftAnimationDto), SlidingOpenAnimationDto.SlidingOpenAnimationDtoType.SlidingLeftAnimationDto)
                .RegisterSubtype(typeof(SlidingRightAnimationDto), SlidingOpenAnimationDto.SlidingOpenAnimationDtoType.SlidingRightAnimationDto)
                .SerializeDiscriminatorProperty().Build());
            settings.Converters.Add(JsonSubtypesConverterBuilder.Of(typeof(SlidingOpenAnimation), "type")
                .RegisterSubtype(typeof(SlidingLeftAnimation), SlidingOpenAnimation.SlidingOpenAnimationType.SlidingLeftAnimation)
                .RegisterSubtype(typeof(SlidingRightAnimation), SlidingOpenAnimation.SlidingOpenAnimationType.SlidingRightAnimation)
                .SerializeDiscriminatorProperty().Build());
        }
    }
}
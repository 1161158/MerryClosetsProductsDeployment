namespace MerryClosets.Models.DTO.DTOValidators
{
    public class AnimationDTOValidator : ValueObjectDTOValidator<AnimationDto>
    {

        public AnimationDTOValidator(){
        }
        public override ValidationOutput DTOIsValid(AnimationDto consideredDto)
        {
            if(consideredDto is FrontalOpenAnimationDto){
                return new ValidationOutputBadRequest();
            }
            if(consideredDto is LateralOpenAnimationDto){
                return new ValidationOutputBadRequest();
            }
            if(consideredDto is SlidingLeftAnimationDto){
                return new ValidationOutputBadRequest();
            }
            if(consideredDto is SlidingRightAnimationDto){
                return new ValidationOutputBadRequest();
            }
            
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            validationOutput.AddError("Inputed animation", "Animation not recongized.");
            return validationOutput;
        }
    }
}
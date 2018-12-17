using System;

namespace MerryClosets.Models.DTO.DTOValidators
{
    public class ComponentDTOValidator : ValueObjectDTOValidator<ComponentDto>
    {
        private readonly AnimationDTOValidator _animationDTOValidator;
        public ComponentDTOValidator(AnimationDTOValidator animationDTOValidator){
            _animationDTOValidator = animationDTOValidator;
        }
        public override ValidationOutput DTOIsValid(ComponentDto consideredDto)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if(!NameIsValid(consideredDto.Name)){
                validationOutput.AddError("Component's name", "Component's name is invalid!");
            }
            var animation = consideredDto.Animation;
            if(animation != null){
                var errors = _animationDTOValidator.DTOIsValid(animation);
                if(errors.HasErrors()){
                    validationOutput.Join(errors);
                }
            }
            return validationOutput;
        }

        private bool NameIsValid(string name)
        {
            return !string.IsNullOrEmpty(name);
        }
    }
}
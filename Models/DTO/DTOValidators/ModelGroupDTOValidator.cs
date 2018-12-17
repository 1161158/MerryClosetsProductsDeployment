using System;
using System.Collections.Generic;

namespace MerryClosets.Models.DTO.DTOValidators
{
    /**
     * Class containing all business rules regarding algorithms.
     */
    public class ModelGroupDTOValidator : ValueObjectDTOValidator<ModelGroupDto>
    {
        private readonly ComponentDTOValidator _componentDTOValidator;
        public ModelGroupDTOValidator(ComponentDTOValidator componentDTOValidator){
            this._componentDTOValidator = componentDTOValidator;
        }
        public override ValidationOutput DTOIsValid(ModelGroupDto consideredDto)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (!RelativeURLIsValid(consideredDto.RelativeURL))
            {
                validationOutput.AddError("RelativeURL", "URL to OBJ file is invalid!");
                return validationOutput;
            }
            var components = consideredDto.Components;
            if(!(components.Count > 0)){
                validationOutput.AddError("Components", "There are no components");
                return validationOutput;
            }
            foreach(var component in components){
                validationOutput = _componentDTOValidator.DTOIsValid(component);
                if(validationOutput.HasErrors()){
                    return validationOutput;
                }
            }
            return validationOutput;
        }


        private bool RelativeURLIsValid(string RelativeURL){
            return !string.IsNullOrEmpty(RelativeURL);
        }
    }
}


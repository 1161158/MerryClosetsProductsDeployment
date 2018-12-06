namespace MerryClosets.Models.DTO.DTOValidators
{
    /**
     * Class containing all business rules regarding colors.
     */
    public class ColorDTOValidator : ValueObjectDTOValidator<ColorDto>
    {
        public ColorDTOValidator() { }

        private bool NameIsValid(string name)
        {
            return !string.IsNullOrEmpty(name);
        }

        private bool DescriptionIsValid(string description)
        {
            return !string.IsNullOrEmpty(description);
        }

        private bool HexCodeIsValid(string hex)
        {
            if (hex.Length==6)
            {
                return !string.IsNullOrEmpty(hex);
            }

            return false;
        }

        public override ValidationOutput DTOIsValid(ColorDto consideredDto)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (!NameIsValid(consideredDto.Name))
            {
                validationOutput.AddError("Color's name", "New name '" + consideredDto.Name + "' is not valid!");
            }
            if (!DescriptionIsValid(consideredDto.Description))
            {
                validationOutput.AddError("Color's description", "Description '" + consideredDto.Description + "' is not valid!");
            }
            if (!HexCodeIsValid(consideredDto.HexCode))
            {
                validationOutput.AddError("Color's hex code", "Hex Code '" + consideredDto.HexCode + "' is not valid!");
            }
            return validationOutput;
        }
        
        public ValidationOutput DTOIsValidForRegister(ColorDto consideredDto)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (!NameIsValid(consideredDto.Name))
            {
                validationOutput.AddError("Name of color", "The name'" + consideredDto.Name + "' is not valid!");
            }
            if (!HexCodeIsValid(consideredDto.HexCode))
            {
                validationOutput.AddError("Hex Code of color", "The hex code'" + consideredDto.HexCode + "' is not valid!");
            }
            return validationOutput;
        }

        public ValidationOutput DTOIsValidForUpdate(ColorDto consideredDto)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (!NameIsValid(consideredDto.Name))
            {
                validationOutput.AddError("Name of color", "New name'" + consideredDto.Name + "' is not valid!");
            }
            if (!DescriptionIsValid(consideredDto.Description))
            {
                validationOutput.AddError("Description of color", "New description'" + consideredDto.Description + "' is not valid!");
            }
            if (!HexCodeIsValid(consideredDto.HexCode))
            {
                validationOutput.AddError("Hex Code", "The hex code '" + consideredDto.HexCode + "' is not valid!");
            }
            return validationOutput;
        }

        public ValidationOutput DTOReferenceIsValid(string colorCode)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (!HexCodeIsValid(colorCode))
            {
                validationOutput.AddError("Hex Code of color", "The hex code '" + colorCode + "' is not valid!");
            } 
            return validationOutput;
        }
    }
}
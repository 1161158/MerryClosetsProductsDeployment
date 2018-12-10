namespace MerryClosets.Models.DTO.DTOValidators
{
    /**
     * Class containing all business rules regarding finishes.
     */
    public class FinishDTOValidator : EntityDTOValidator<FinishDto>
    {
        private readonly PriceDTOValidator _priceDTOValidator;

        public FinishDTOValidator(PriceDTOValidator priceDTOValidator)
        {
            _priceDTOValidator = priceDTOValidator;
        }

        private bool NameIsValid(string name)
        {
            return !string.IsNullOrEmpty(name);
        }

        private bool DescriptionIsValid(string description)
        {
            return !string.IsNullOrEmpty(description);
        }

        public override ValidationOutput DTOIsValidForRegister(FinishDto consideredDto)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (!NameIsValid(consideredDto.Name))
            {
                validationOutput.AddError("Name of finish", "Name '" + consideredDto.Name + "' is invalid!");
            }
            if (!DescriptionIsValid(consideredDto.Description))
            {
                validationOutput.AddError("Description of finish", "Description '" + consideredDto.Description + "' is invalid!");
            }

            if (consideredDto.Price != null)
            {
                ValidationOutput priceDTOValidationOutput = _priceDTOValidator.DTOIsValid(consideredDto.Price);
                if (priceDTOValidationOutput.HasErrors())
                {
                    priceDTOValidationOutput.AppendToAllkeys("Finish '" + consideredDto.Reference + "' | ");
                    validationOutput.Join(priceDTOValidationOutput);
                }
            }
            else
            {
                validationOutput.AddError("Price of finish", "The price has can not be null!");
            }
            return validationOutput;
        }

        public override ValidationOutput DTOIsValidForUpdate(FinishDto consideredDto)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (!ReferenceIsValid(consideredDto.Reference))
            {
                validationOutput.AddError("Reference of finish", "New reference '" + consideredDto.Reference + "' is invalid!");
            }
            if (!NameIsValid(consideredDto.Name))
            {
                validationOutput.AddError("Name of finish", "New name '" + consideredDto.Name + "' is invalid!");
            }
            if (!DescriptionIsValid(consideredDto.Description))
            {
                validationOutput.AddError("Description of finish", "New description '" + consideredDto.Description + "' is invalid!");
            }
            if (consideredDto.Price != null)
            {
                ValidationOutput priceDTOValidationOutput = _priceDTOValidator.DTOIsValid(consideredDto.Price);
                if (priceDTOValidationOutput.HasErrors())
                {
                    priceDTOValidationOutput.AppendToAllkeys("Finish '" + consideredDto.Reference + "' | ");
                    validationOutput.Join(priceDTOValidationOutput);
                }
            }
            else
            {
                validationOutput.AddError("Price of finish", "The price has can not be null!");
            }
            return validationOutput;
        }

        public override ValidationOutput DTOReferenceIsValid(string finishReference)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (!ReferenceIsValid(finishReference))
            {
                validationOutput.AddError("Reference of finish", "The reference '" + finishReference + "' is not valid!");
            }
            return validationOutput;
        }
    }
}
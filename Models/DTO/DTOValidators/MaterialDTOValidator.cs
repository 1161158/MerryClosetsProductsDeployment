
namespace MerryClosets.Models.DTO.DTOValidators
{
    /**
     * Class containing all business rules regarding materials.
     */
    public class MaterialDTOValidator : EntityDTOValidator<MaterialDto>
    {
        private readonly PriceDTOValidator _priceDTOValidator;

        public MaterialDTOValidator(PriceDTOValidator priceDTOValidator)
        {
            _priceDTOValidator = priceDTOValidator;
        }

        protected bool NameIsValid(string name)
        {
            return !string.IsNullOrEmpty(name);
        }

        protected bool DescriptionIsValid(string description)
        {
            return !string.IsNullOrEmpty(description);
        }

        public override ValidationOutput DTOIsValidForRegister(MaterialDto consideredDto)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (!NameIsValid(consideredDto.Name))
            {
                validationOutput.AddError("Name of material", "The name'" + consideredDto.Name + "' is not valid!");
            }
            if (!DescriptionIsValid(consideredDto.Description))
            {
                validationOutput.AddError("Description of material", "The description'" + consideredDto.Description + "' is not valid!");
            }

            if (consideredDto.Price != null)
            {
                ValidationOutput priceDTOValidationOutput = _priceDTOValidator.DTOIsValid(consideredDto.Price);
                if (priceDTOValidationOutput.HasErrors())
                {
                    priceDTOValidationOutput.AppendToAllkeys("Material '" + consideredDto.Reference + "' > ");
                    validationOutput.Join(priceDTOValidationOutput);
                }
            }
            else
            {
                validationOutput.AddError("Price of material", "The price has can not be null!");
            }

            return validationOutput;
        }

        public override ValidationOutput DTOIsValidForUpdate(MaterialDto consideredDto)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (consideredDto.Name != null)
            {
                if (!NameIsValid(consideredDto.Name))
                {
                    validationOutput.AddError("Name of material", "New name'" + consideredDto.Name + "' is not valid!");
                }
            }

            if (consideredDto.Description != null)
            {
                if (!DescriptionIsValid(consideredDto.Description))
                {
                    validationOutput.AddError("Description of material",
                        "New description'" + consideredDto.Description + "' is not valid!");
                }
            }

            if (consideredDto.Price != null)
            {
                ValidationOutput priceDTOValidationOutput = _priceDTOValidator.DTOIsValid(consideredDto.Price);
                if (priceDTOValidationOutput.HasErrors())
                {
                    priceDTOValidationOutput.AppendToAllkeys("Material '" + consideredDto.Reference + "' > ");
                    validationOutput.Join(priceDTOValidationOutput);
                }
            }
            return validationOutput;
        }

        public override ValidationOutput DTOReferenceIsValid(string materialReference)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (!ReferenceIsValid(materialReference))
            {
                validationOutput.AddError("Reference of material", "The reference '" + materialReference + "' is not valid!");
            }
            return validationOutput;
        }
    }
}
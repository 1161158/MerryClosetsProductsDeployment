namespace MerryClosets.Models.DTO.DTOValidators
{
    /**
     * Class containing all business rules regarding products.
     */
    public class ProductDTOValidator : EntityDTOValidator<ProductDto>
    {
        private readonly SlotDefinitionDTOValidator _slotDefinitionDTOValidator;
        private readonly PriceDTOValidator _priceDTOValidator;
        private readonly DimensionValuesDTOValidator _dimensionValuesDTOValidator;

        public ProductDTOValidator(SlotDefinitionDTOValidator slotDefinitionDTOValidator,
            PriceDTOValidator priceDTOValidator, DimensionValuesDTOValidator dimensionValuesDTOValidator)
        {
            _priceDTOValidator = priceDTOValidator;
            _slotDefinitionDTOValidator = slotDefinitionDTOValidator;
            _dimensionValuesDTOValidator = dimensionValuesDTOValidator;
        }

        private bool NameIsValid(string name)
        {
            return !string.IsNullOrEmpty(name);
        }

        private bool DescriptionIsValid(string description)
        {
            return !string.IsNullOrEmpty(description);
        }

        public override ValidationOutput DTOIsValidForRegister(ProductDto consideredDto)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (!NameIsValid(consideredDto.Name))
            {
                validationOutput.AddError("Name of product", "The name'" + consideredDto.Name + "' is not valid!");
            }

            if (!DescriptionIsValid(consideredDto.Description))
            {
                validationOutput.AddError("Description of product",
                    "The description'" + consideredDto.Description + "' is not valid!");
            }

            ValidationOutput priceDTOValidationOutput = _priceDTOValidator.DTOIsValid(consideredDto.Price);
            if (priceDTOValidationOutput.HasErrors())
            {
                priceDTOValidationOutput.AppendToAllkeys("Product '" + consideredDto.Reference + "' > ");
                validationOutput.Join(priceDTOValidationOutput);
            }

            ValidationOutput slotDefinitionDTOValidationOutput =
                _slotDefinitionDTOValidator.DTOIsValid(consideredDto.SlotDefinition);
            if (slotDefinitionDTOValidationOutput.HasErrors())
            {
                slotDefinitionDTOValidationOutput.AppendToAllkeys("Product '" + consideredDto.Reference + "' > ");
                validationOutput.Join(slotDefinitionDTOValidationOutput);
            }

            return validationOutput;
        }

        public override ValidationOutput DTOIsValidForUpdate(ProductDto consideredDto)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (!NameIsValid(consideredDto.Name))
            {
                validationOutput.AddError("Name of product", "New name '" + consideredDto.Name + "' is not valid!");
            }

            if (!DescriptionIsValid(consideredDto.Description))
            {
                validationOutput.AddError("Description of product",
                    "New description '" + consideredDto.Description + "' is not valid!");
            }

            ValidationOutput priceDTOValidationOutput = _priceDTOValidator.DTOIsValid(consideredDto.Price);
            if (priceDTOValidationOutput.HasErrors())
            {
                priceDTOValidationOutput.AppendToAllkeys("Product '" + consideredDto.Reference + "' > ");
                validationOutput.Join(priceDTOValidationOutput);
            }

            ValidationOutput slotDefinitionDTOValidationOutput =
                _slotDefinitionDTOValidator.DTOIsValid(consideredDto.SlotDefinition);
            if (slotDefinitionDTOValidationOutput.HasErrors())
            {
                slotDefinitionDTOValidationOutput.AppendToAllkeys("Product '" + consideredDto.Reference + "' > ");
                validationOutput.Join(slotDefinitionDTOValidationOutput);
            }

            return validationOutput;
        }

        public override ValidationOutput DTOReferenceIsValid(string materialReference)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (!ReferenceIsValid(materialReference))
            {
                validationOutput.AddError("Reference of product",
                    "The reference '" + materialReference + "' is not valid!");
            }

            return validationOutput;
        }
    }
}
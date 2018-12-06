using System.Collections.Generic;
using System.Linq;
using MerryClosets.Models.Collection;

namespace MerryClosets.Models.DTO.DTOValidators
{
    /**
     * Class containing all business rules regarding configured products.
     */
    public class ConfiguredProductDTOValidator : EntityDTOValidator<ConfiguredProductDto>
    {

        private readonly ColorDTOValidator _colorDTOValidator;
        private readonly FinishDTOValidator _finishDTOValidator;
        private readonly MaterialDTOValidator _materialDTOValidator;

        public ConfiguredProductDTOValidator(ColorDTOValidator colorDTOValidator, FinishDTOValidator finishDTOValidator, MaterialDTOValidator materialDTOValidator)
        {
            _colorDTOValidator = colorDTOValidator;
            _finishDTOValidator = finishDTOValidator;
            _materialDTOValidator = materialDTOValidator;
        }

        public override ValidationOutput DTOIsValidForRegister(ConfiguredProductDto consideredDto)
        {
            /* foreach(var part in dto.Parts){
                if(!part.ModeRestrictionIsValid(part.ModelRestriction)){
                    return false;
                }
            }*/
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            ConfiguredMaterialDtoIsValid(consideredDto.ConfiguredMaterial, validationOutput);

            ConfiguredDimensionDtoIsValid(consideredDto.ConfiguredDimension, validationOutput);
            ProductReferenceIsValid(consideredDto.ProductReference, validationOutput);
            return validationOutput;
        }

        private void ProductReferenceIsValid(string productReference, ValidationOutput validationOutput)
        {
            if (productReference == null)
            {
                validationOutput.AddError("Product's reference", "Product's reference is missing!");
            }
        }

        private void ConfiguredDimensionDtoIsValid(ConfiguredDimensionDto dto, ValidationOutput validationOutput)
        {
            if (dto == null)
            {
                validationOutput.AddError("Dimensions", "Dimensions are missing!");
                return;
            }
            if (dto.Depth <= 0)
            {
                validationOutput.AddError("Depth", "Depth is less or equals to 0");
            }
            if (dto.Height <= 0)
            {
                validationOutput.AddError("Height", "Height is less or equals to 0");
            }
            if (dto.Width <= 0)
            {
                validationOutput.AddError("Width", "Width is less or equals to 0");
            }
        }

        private void ConfiguredMaterialDtoIsValid(ConfiguredMaterialDto dto, ValidationOutput validationOutput)
        {
            if (dto == null)
            {
                validationOutput.AddError("Material", "Material is missing!");
                return;
            }
            if (dto.OriginMaterialReference == null)
            {
                validationOutput.AddError("Material's reference", "Material's reference is missing!");
            }
            if (dto.ColorReference == null)
            {
                validationOutput.AddError("Color", "Color is missing!");
                return;
            }
            if (dto.ColorReference == null)
            {
                validationOutput.AddError("Color's reference", "Color's reference is missing!");
            }
            if (dto.FinishReference == null)
            {
                validationOutput.AddError("Finish", "Finish is missing!");
                return;
            }
            if (dto.FinishReference == null)
            {
                validationOutput.AddError("Finish's reference", "Finish's reference is missing!");
            }
        }

        public override ValidationOutput DTOIsValidForUpdate(ConfiguredProductDto consideredDto)
        {
            return new ValidationOutputBadRequest();
        }

        public override ValidationOutput DTOReferenceIsValid(string configuredProductReference)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (!ReferenceIsValid(configuredProductReference))
            {
                validationOutput.AddError("Configured product reference", "Configured product reference is invalid!");
            }
            return validationOutput;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using MerryClosets.Models.Collection;

namespace MerryClosets.Models.DTO.DTOValidators
{
    /**
     * Class containing all business rules regarding catalogs.
     */
    public class CatalogDTOValidator : EntityDTOValidator<CatalogDto>
    {

        public CatalogDTOValidator() { }

        private bool NameIsValid(string name)
        {
            return !string.IsNullOrEmpty(name);
        }

        private bool DescriptionIsValid(string description)
        {
            return !string.IsNullOrEmpty(description);
        }

        public override ValidationOutput DTOIsValidForRegister(CatalogDto consideredDto)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (!NameIsValid(consideredDto.Name))
            {
                validationOutput.AddError("Name of catalog", "The name'" + consideredDto.Name + "' is not valid!");
            }
            if (!DescriptionIsValid(consideredDto.Description))
            {
                validationOutput.AddError("Description of catalog", "The description'" + consideredDto.Description + "' is not valid!");
            }
            return validationOutput;
        }

        public override ValidationOutput DTOIsValidForUpdate(CatalogDto consideredDto)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (!NameIsValid(consideredDto.Name))
            {
                validationOutput.AddError("Name of catalog", "New name'" + consideredDto.Name + "' is not valid!");
            }
            if (!DescriptionIsValid(consideredDto.Description))
            {
                validationOutput.AddError("Description of catalog", "New description'" + consideredDto.Description + "' is not valid!");
            }
            return validationOutput;
        }

        public override ValidationOutput DTOReferenceIsValid(string catalogReference)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (!ReferenceIsValid(catalogReference))
            {
                validationOutput.AddError("Reference of catalog", "The reference '" + catalogReference + "' is not valid!");
            }
            return validationOutput;
        }
    }
}
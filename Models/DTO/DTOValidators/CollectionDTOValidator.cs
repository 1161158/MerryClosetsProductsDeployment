using System.Collections.Generic;
using System.Linq;
using MerryClosets.Models.Collection;

namespace MerryClosets.Models.DTO.DTOValidators
{
    /**
     * Class containing all business rules regarding collections.
     */
    public class CollectionDTOValidator : EntityDTOValidator<CollectionDto>
    {

        public CollectionDTOValidator() { }

        private bool NameIsValid(string name)
        {
            return !string.IsNullOrEmpty(name);
        }

        private bool DescriptionIsValid(string description)
        {
            return !string.IsNullOrEmpty(description);
        }

        public override ValidationOutput DTOIsValidForRegister(CollectionDto consideredDto)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (consideredDto.Name != null)
            {
                if (!NameIsValid(consideredDto.Name))
                {
                    validationOutput.AddError("Name of collection",
                        "The name'" + consideredDto.Name + "' is not valid!");
                }
            }

            if (consideredDto.Description != null)
            {
                if (!DescriptionIsValid(consideredDto.Description))
                {
                    validationOutput.AddError("Description of collection",
                        "The description'" + consideredDto.Description + "' is not valid!");
                }
            }

            return validationOutput;
        }

        public override ValidationOutput DTOIsValidForUpdate(CollectionDto consideredDto)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (!NameIsValid(consideredDto.Name))
            {
                validationOutput.AddError("Name of collection", "New name'" + consideredDto.Name + "' is not valid!");
            }
            if (!DescriptionIsValid(consideredDto.Description))
            {
                validationOutput.AddError("Description of collection", "New description'" + consideredDto.Description + "' is not valid!");
            }
            return validationOutput;
        }

        public override ValidationOutput DTOReferenceIsValid(string collectionReference)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (!ReferenceIsValid(collectionReference))
            {
                validationOutput.AddError("Reference of collection", "The reference '" + collectionReference + "' is not valid!");
            }
            return validationOutput;
        }
    }
}
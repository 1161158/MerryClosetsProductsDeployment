namespace MerryClosets.Models.DTO.DTOValidators
{
    /**
     * Class containing all business rules regarding categories.
     */
    public class CategoryDTOValidator : EntityDTOValidator<CategoryDto>
    {

        public CategoryDTOValidator() { }

        private bool NameIsValid(string name)
        {
            return !string.IsNullOrEmpty(name);
        }

        private bool DescriptionIsValid(string description)
        {
            return !string.IsNullOrEmpty(description);
        }

        private bool ParentCategoryReferenceIsValid(string parentCategoryReference)
        {
            if (parentCategoryReference != null)
            {
                if (parentCategoryReference.Length == 0)
                {
                    return false;
                }
            }
            return true;
        }

        public override ValidationOutput DTOIsValidForRegister(CategoryDto consideredDto)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (!NameIsValid(consideredDto.Name))
            {
                validationOutput.AddError("Name of category", "The name'" + consideredDto.Name + "' is not valid!");
            }
            if (!DescriptionIsValid(consideredDto.Description))
            {
                validationOutput.AddError("Description of category", "The description'" + consideredDto.Description + "' is not valid!");
            }
            if (!ParentCategoryReferenceIsValid(consideredDto.ParentCategoryReference))
            {
                validationOutput.AddError("Parent category reference of category", "The parent category reference '" + consideredDto.ParentCategoryReference + "' is not valid!");
            }
            return validationOutput;
        }

        public override ValidationOutput DTOIsValidForUpdate(CategoryDto consideredDto)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (!NameIsValid(consideredDto.Name))
            {
                validationOutput.AddError("Name of category", "New name '" + consideredDto.Name + "' is not valid!");
            }
            if (!DescriptionIsValid(consideredDto.Description))
            {
                validationOutput.AddError("Description of category", "New description '" + consideredDto.Description + "' is not valid!");
            }
            return validationOutput;
        }

        public override ValidationOutput DTOReferenceIsValid(string categoryReference)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (!ReferenceIsValid(categoryReference))
            {
                validationOutput.AddError("Reference of category", "The reference '" + categoryReference + "' is not valid!");
            }
            return validationOutput;
        }
    }
}
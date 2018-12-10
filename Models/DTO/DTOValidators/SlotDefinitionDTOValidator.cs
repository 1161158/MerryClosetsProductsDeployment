namespace MerryClosets.Models.DTO.DTOValidators
{
    /**
     * Class containing all business rules regarding slot definitions.
     */
    public class SlotDefinitionDTOValidator : ValueObjectDTOValidator<SlotDefinitionDto>
    {
        public override ValidationOutput DTOIsValid(SlotDefinitionDto consideredDto)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (consideredDto != null && (consideredDto.MinSize <= 0 || consideredDto.MaxSize <= 0 ||
                                          consideredDto.MaxSize < consideredDto.MinSize ||
                                          consideredDto.RecSize > consideredDto.MaxSize ||
                                          consideredDto.RecSize < consideredDto.MinSize))
            {
                validationOutput.AddError("Slot definition", "The slot definition is not valid!");
            }

            return validationOutput;
        }
    }
}
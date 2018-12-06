namespace MerryClosets.Models.DTO.DTOValidators
{
    /**
     * Class containing all business rules regarding colors.
     */
    public class DiscreteValueDTOValidator : ValueObjectDTOValidator<DiscreteValueDto>
    {

        public DiscreteValueDTOValidator() { }

        public override ValidationOutput DTOIsValid(DiscreteValueDto consideredDto)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (consideredDto.Value <= 0)
            {
                validationOutput.AddError("Discrete value", " A discrete value is not valid!");
            }
            return validationOutput;
        }

    }
}
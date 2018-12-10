namespace MerryClosets.Models.DTO.DTOValidators
{
    /**
     * Class containing all business rules regarding colors.
     */
    public class ContinuousValueDTOValidator : ValueObjectDTOValidator<ContinuousValueDto>
    {

        public ContinuousValueDTOValidator() { }

        public override ValidationOutput DTOIsValid(ContinuousValueDto consideredDto)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (consideredDto.MinValue <= 0 || consideredDto.MinValue > consideredDto.MaxValue)
            {
                validationOutput.AddError("Continuous value", " A continuous value is not valid!");
            }
            return validationOutput;
        }

    }
}
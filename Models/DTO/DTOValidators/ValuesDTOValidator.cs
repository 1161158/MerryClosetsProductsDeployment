namespace MerryClosets.Models.DTO.DTOValidators
{
    /**
     * Class containing all business rules regarding colors.
     */
    public class ValuesDTOValidator : ValueObjectDTOValidator<ValuesDto>
    {
        private readonly ContinuousValueDTOValidator _continuousValueDTOValidator;
        private readonly DiscreteValueDTOValidator _discreteValueDTOValidator;

        public ValuesDTOValidator(DiscreteValueDTOValidator discreteValueDTOValidator, ContinuousValueDTOValidator continuousValueDTOValidator)
        {
            _continuousValueDTOValidator = continuousValueDTOValidator;
            _discreteValueDTOValidator = discreteValueDTOValidator;
        }

        public override ValidationOutput DTOIsValid(ValuesDto consideredDto)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (consideredDto is DiscreteValueDto)
            {
                validationOutput = _discreteValueDTOValidator.DTOIsValid((DiscreteValueDto)consideredDto);
                if (validationOutput.HasErrors())
                {
                    return validationOutput;
                }
            }

            if (consideredDto is ContinuousValueDto)
            {
                validationOutput = _continuousValueDTOValidator.DTOIsValid((ContinuousValueDto)consideredDto);
                if (validationOutput.HasErrors())
                {
                    return validationOutput;
                }
            }
            return validationOutput;
        }
    }
}
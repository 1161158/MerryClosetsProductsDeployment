namespace MerryClosets.Models.DTO.DTOValidators
{
    /**
     * Class containing all business rules regarding prices.
     */
    public class PriceDTOValidator : ValueObjectDTOValidator<PriceDto>
    {

        public PriceDTOValidator() { }

        private bool ValueIsValid(float value)
        {
            return value >= 0;
        }

        public override ValidationOutput DTOIsValid(PriceDto consideredDto)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (!ValueIsValid(consideredDto.Value))
            {
                validationOutput.AddError("Price", "Inputed price is invalid!");
            }
            return validationOutput;
        }
    }
}
using System;

namespace MerryClosets.Models.DTO.DTOValidators
{
    /**
     * Class containing all business rules regarding price history items.
     */
    public class PriceHistoryDTOValidator : ValueObjectDTOValidator<PriceHistoryDto>
    {
        public PriceHistoryDTOValidator()
        {
        }

        public override ValidationOutput DTOIsValid(PriceHistoryDto consideredDto)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (consideredDto.Date < DateTime.Now)
            {
                validationOutput.AddError("Price history item", "A price history item has a date that has already passed.");
                return validationOutput;
            }
            return validationOutput;
        }
    }
}
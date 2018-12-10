using System;

namespace MerryClosets.Models.DTO.DTOValidators
{
    /**
     * Class containing all business rules regarding slot definitions.
     */
    public class SizePercentagePartAlgorithmDTOValidator : ValueObjectDTOValidator<SizePercentagePartAlgorithmDto>
    {

        public override ValidationOutput DTOIsValid(SizePercentagePartAlgorithmDto consideredDto)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (consideredDto.Min <= 0 || consideredDto.Max <= 0 || consideredDto.Min > 100 || consideredDto.Max > 100 || consideredDto.Max < consideredDto.Min
             || (!string.Equals(consideredDto.SizeType, "height", StringComparison.Ordinal) && !string.Equals(consideredDto.SizeType, "depth", StringComparison.Ordinal) && !string.Equals(consideredDto.SizeType, "width", StringComparison.Ordinal)))
            {
                validationOutput.AddError("SizePercentagePart's attribute", "SizePercentagePart's attribute not valid!");
            }
            return validationOutput;
        }
    }
}
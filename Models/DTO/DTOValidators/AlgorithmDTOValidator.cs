namespace MerryClosets.Models.DTO.DTOValidators
{
    /**
     * Class containing all business rules regarding algorithms.
     */
    public class AlgorithmDTOValidator : ValueObjectDTOValidator<AlgorithmDto>
    {

        private readonly RatioAlgorithmDTOValidator _ratioAlgorithmDTOValidator;
        private readonly SizePercentagePartAlgorithmDTOValidator _sizePercentagePartAlgorithmDTOValidator;

        public AlgorithmDTOValidator(RatioAlgorithmDTOValidator ratioAlgorithmDTOValidator, SizePercentagePartAlgorithmDTOValidator sizePercentagePartAlgorithmDTOValidator)
        {
            _ratioAlgorithmDTOValidator = ratioAlgorithmDTOValidator;
            _sizePercentagePartAlgorithmDTOValidator = sizePercentagePartAlgorithmDTOValidator;
        }

        public override ValidationOutput DTOIsValid(AlgorithmDto consideredDto)
        {
            if(consideredDto is MaterialFinishPartAlgorithmDto){
                return new ValidationOutputBadRequest();
            }
            if(consideredDto is MaterialPartAlgorithmDto){
                return new ValidationOutputBadRequest();
            }
            if (consideredDto is RatioAlgorithmDto)
            {
                return _ratioAlgorithmDTOValidator.DTOIsValid((RatioAlgorithmDto)consideredDto);
            }
            if(consideredDto is SizePartAlgorithmDto){
                return new ValidationOutputBadRequest();
            }
            if(consideredDto is SizePercentagePartAlgorithmDto){
                return _sizePercentagePartAlgorithmDTOValidator.DTOIsValid((SizePercentagePartAlgorithmDto) consideredDto);
            }
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            validationOutput.AddError("Inputed algorithm", "Algorithm not recongized.");
            return validationOutput;
        }
    }
}


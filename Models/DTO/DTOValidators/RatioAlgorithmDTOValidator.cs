using System.Collections.Generic;

namespace MerryClosets.Models.DTO.DTOValidators
{
    /**
     * Class containing all business rules regarding colors.
     */
    public class RatioAlgorithmDTOValidator : ValueObjectDTOValidator<RatioAlgorithmDto>
    {

        private static readonly float _maxRatioValue = 100;
        private static readonly float _minRatioValue = 0;
        private static readonly List<string> _operators = new List<string>{ ">", "<", ">=", "<=" };
        private static readonly List<string> _dimensions = new List<string>{ "height", "width", "depth" };

        public RatioAlgorithmDTOValidator() { }

        public static bool ValueDescIsValid(string valueDesc)
        {
            if (string.IsNullOrEmpty(valueDesc) || !_dimensions.Contains(valueDesc))
            {
                return false;
            }
            return true;
        }

        public static bool OperatorIsValid(string op)
        {
            if (string.IsNullOrEmpty(op) || !_operators.Contains(op))
            {
                return false;
            }
            return true;
        }

        public static bool RatioIsValid(float ratio)
        {
            if (ratio < _minRatioValue || ratio > _maxRatioValue)
            {
                return false;
            }
            return true;
        }

        public override ValidationOutput DTOIsValid(RatioAlgorithmDto consideredDto)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();
            if (consideredDto == null)
            {
                validationOutput.AddError("Ratio algorithm", "Null.");
                return validationOutput;
            }

            if (!ValueDescIsValid(consideredDto.FirstValueDesc))
            {
                validationOutput.AddError("Ratio algorithm's First Value", "Inputed value description '" + consideredDto.FirstValueDesc + "' is not valid!");
            }

            if (!ValueDescIsValid(consideredDto.SecondValueDesc))
            {
                validationOutput.AddError("Ratio algorithm's Second Value", "Inputed value description '" + consideredDto.SecondValueDesc + "' is not valid!");
            }

            if (!OperatorIsValid(consideredDto.Operator))
            {
                validationOutput.AddError("Ratio algorithm's Operator", "Inputed operator '" + consideredDto.Operator + "' is not valid!");
            }

            if (!RatioIsValid(consideredDto.Ratio))
            {
                validationOutput.AddError("Ratio algorithm's Ratio", "Inputed ratio '" + consideredDto.Ratio + "' is not valid!");
            }

            return validationOutput;
        }
    }
}
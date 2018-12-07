
using System.Collections.Generic;
using MerryClosets.Models.Product;

namespace MerryClosets.Models.DTO.DTOValidators
{
    /**
     * Class containing all business rules regarding colors.
     */
    public class DimensionValuesDTOValidator : ValueObjectDTOValidator<DimensionValuesDto>
    {
        private readonly ValuesDTOValidator _valuesDTOValidator;

        public DimensionValuesDTOValidator(ValuesDTOValidator valuesDTOValidator)
        {
            _valuesDTOValidator = valuesDTOValidator;
        }

        public override ValidationOutput DTOIsValid(DimensionValuesDto consideredDto)
        {
            ValidationOutput validationOutput = new ValidationOutputBadRequest();

            //================== PossibleHeights attribute ==================
            if (consideredDto.PossibleHeights == null || consideredDto.PossibleHeights.Count <= 0)
            {
                validationOutput.AddError("Dimension's heights", "There are no heights.");
                return validationOutput;
            }

            List<ValuesDto> possibleHeights = new List<ValuesDto>();
            foreach (var setOfValuesDto in consideredDto.PossibleHeights)
            {
                ValidationOutput eachValueValidationOutput = _valuesDTOValidator.DTOIsValid(setOfValuesDto);
                if (eachValueValidationOutput.HasErrors())
                {
                    validationOutput.Join(eachValueValidationOutput);
                    return validationOutput;
                }

                validationOutput = new ValidationOutputBadRequest();
                if (possibleHeights.Contains(setOfValuesDto))
                {
                    if (setOfValuesDto is ContinuousValueDto)
                    {
                        ContinuousValueDto temp = (ContinuousValueDto)setOfValuesDto;
                        validationOutput.AddError("Dimension's heights", "The height interval [" + temp.MinValue + ", " + temp.MaxValue + "] is duplicated in the list of possible heights.");
                    }
                    if (setOfValuesDto is DiscreteValueDto)
                    {
                        validationOutput.AddError("Dimension's heights", "The height '" + setOfValuesDto + "' is duplicated in the list of possible heights.");
                    }
                    return validationOutput;
                }

                possibleHeights.Add(setOfValuesDto);
            }
            if (validationOutput.HasErrors())
            {
                return validationOutput;
            }

            //================== PossibleWidths attribute ==================
            if (consideredDto.PossibleWidths == null || consideredDto.PossibleWidths.Count <= 0)
            {
                validationOutput.AddError("Dimension's widths", "There are no widths.");
                return validationOutput;
            }

            List<ValuesDto> possibleWidths = new List<ValuesDto>();
            foreach (var setOfValuesDto in consideredDto.PossibleWidths)
            {
                ValidationOutput eachValueValidationOutput = _valuesDTOValidator.DTOIsValid(setOfValuesDto);
                if (eachValueValidationOutput.HasErrors())
                {
                    validationOutput.Join(eachValueValidationOutput);
                    return validationOutput;
                }

                validationOutput = new ValidationOutputBadRequest();
                if (possibleWidths.Contains(setOfValuesDto))
                {
                    if (setOfValuesDto is ContinuousValueDto)
                    {
                        ContinuousValueDto temp = (ContinuousValueDto)setOfValuesDto;
                        validationOutput.AddError("Dimension's widths", "The width interval [" + temp.MinValue + ", " + temp.MaxValue + "] is duplicated in the list of possible widths.");
                    }
                    if (setOfValuesDto is DiscreteValueDto)
                    {
                        validationOutput.AddError("Dimension's widths", "The width '" + setOfValuesDto + "' is duplicated in the list of possible widths.");
                    }
                    return validationOutput;
                }

                possibleWidths.Add(setOfValuesDto);
            }
            if (validationOutput.HasErrors())
            {
                return validationOutput;
            }

            //================== PossibleDepths attribute ==================
            if (consideredDto.PossibleDepths == null || consideredDto.PossibleDepths.Count <= 0)
            {
                validationOutput.AddError("Dimension's depths", "There are no depths");
                return validationOutput;
            }

            List<ValuesDto> possibleDepths = new List<ValuesDto>();
            foreach (var setOfValuesDto in consideredDto.PossibleDepths)
            {
                ValidationOutput eachValueValidationOutput = _valuesDTOValidator.DTOIsValid(setOfValuesDto);
                if (eachValueValidationOutput.HasErrors())
                {
                    validationOutput.Join(eachValueValidationOutput);
                    return validationOutput;
                }

                validationOutput = new ValidationOutputBadRequest();
                if (possibleDepths.Contains(setOfValuesDto))
                {
                    if (setOfValuesDto is ContinuousValueDto)
                    {
                        ContinuousValueDto temp = (ContinuousValueDto)setOfValuesDto;
                        validationOutput.AddError("Dimension's depths", "The depth interval [" + temp.MinValue + ", " + temp.MaxValue + "] is duplicated in the list of possible depths.");
                    }
                    if (setOfValuesDto is DiscreteValueDto)
                    {
                        validationOutput.AddError("Dimension's depths", "The depth '" + setOfValuesDto + "' is duplicated in the list of possible depths.");
                    }
                    return validationOutput;
                }

                possibleDepths.Add(setOfValuesDto);
            }

            return validationOutput;
        }
    }
}

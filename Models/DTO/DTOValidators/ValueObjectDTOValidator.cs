namespace MerryClosets.Models.DTO.DTOValidators
{
    /**
     * Interface to be implemented by each validator, one for each value object of the model.
     */
    public abstract class ValueObjectDTOValidator<T> where T : ValueObjectDto
    {
        public abstract ValidationOutput DTOIsValid(T consideredDto);
    }
}
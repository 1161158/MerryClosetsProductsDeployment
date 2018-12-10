namespace MerryClosets.Models.DTO.DTOValidators
{
    /**
     * Interface to be implemented by each validator, one for each entity of the model.
     */
    public abstract class EntityDTOValidator<T> where T : BaseEntityDto
    {
        public abstract ValidationOutput DTOIsValidForRegister(T consideredDto);

        public abstract ValidationOutput DTOIsValidForUpdate(T consideredDto);

        public abstract ValidationOutput DTOReferenceIsValid(string consideredReference);

        protected bool ReferenceIsValid(string reference)
        {
            return !string.IsNullOrEmpty(reference);
        }
    }
}
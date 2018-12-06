namespace MerryClosets.Models.DTO
{
    public abstract class BaseEntityDto
    {
        public long Id { get; set; }

        public bool IsActive { get; set; }

        public string Reference { get; set; }
        
        public override string ToString()
        {
            return this.Reference;
        }
    }
}
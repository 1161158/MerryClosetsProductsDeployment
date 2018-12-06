namespace MerryClosets.Models.DTO
{
    public class CategoryDto : BaseEntityDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ParentCategoryReference { get; set; }

        public CategoryDto(string name, string description, string reference, string parentCategoryReference)
        {
            this.Name = name;
            this.Description = description;
            this.Reference = reference;
            this.ParentCategoryReference = parentCategoryReference;
        }

        public CategoryDto(string name, string description, string reference)
        {
            this.Name = name;
            this.Description = description;
            this.Reference = reference;
            this.ParentCategoryReference = null;
        }

        protected CategoryDto() { }
    }
}
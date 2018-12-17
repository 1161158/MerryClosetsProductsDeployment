namespace MerryClosets.Models.DTO
{
    public class CategoryDto : BaseEntityDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ParentCategoryReference { get; set; }
        public bool IsExternal { get; set; }

        public CategoryDto(string name, string description, string reference, string parentCategoryReference)
        {
            this.Name = name;
            this.Description = description;
            this.Reference = reference;
            this.ParentCategoryReference = parentCategoryReference;
            this.IsExternal = false;
        }
        
        public CategoryDto(string name, string description, string reference, string parentCategoryReference, bool isExternal)
        {
            this.Name = name;
            this.Description = description;
            this.Reference = reference;
            this.ParentCategoryReference = parentCategoryReference;
            this.IsExternal = isExternal;
        }

        public CategoryDto(string name, string description, string reference, bool isExternal)
        {
            this.Name = name;
            this.Description = description;
            this.Reference = reference;
            this.ParentCategoryReference = null;
            this.IsExternal = isExternal;
        }
        
        public CategoryDto(string name, string description, string reference)
        {
            this.Name = name;
            this.Description = description;
            this.Reference = reference;
            this.ParentCategoryReference = null;
            this.IsExternal = false;
        }

        protected CategoryDto() { }
    }
}
using System;
using MerryClosets.Models.DTO;

namespace MerryClosets.Models.Category
{
    public class Category : BaseEntity
    {
        /**
         * Name of the category.
         */
        public string Name { get; set; }

        /**
         * Description of what type of elements the category holds.
         */
        public string Description { get; set; }

        /**
         * Reference of the parent category. Can be null, indicating that it's a root category.
         */
        public string ParentCategoryReference { get; set; }
        
        public bool IsExternal { get; set; }

        public Category(string name, string description, string reference, string parentCategoryReference)
        {
            this.Name = name;
            this.Description = description;
            this.Reference = reference;
            this.ParentCategoryReference = parentCategoryReference;
            this.IsExternal = false;
        }
        
        public Category(string name, string description, string reference, string parentCategoryReference, bool isExternal)
        {
            this.Name = name;
            this.Description = description;
            this.Reference = reference;
            this.ParentCategoryReference = parentCategoryReference;
            this.IsExternal = isExternal;
        }
        
        public Category(string name, string description, string reference, bool isExternal)
        {
            this.Name = name;
            this.Description = description;
            this.Reference = reference;
            this.ParentCategoryReference = null;
            this.IsExternal = isExternal;
        }

        public Category(string name, string description, string reference)
        {
            this.Name = name;
            this.Description = description;
            this.Reference = reference;
            this.ParentCategoryReference = null;
            this.IsExternal = false;
        }

        /**
        * Overriden Equals method.
        * Compares the two objects. The instances of Category are equal if the reference is the same.
        */
        public override bool Equals(Object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }
            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }
            Category c = obj as Category;
            return string.Equals(this.Reference, c.Reference, StringComparison.Ordinal);
        }

        /**
         * Overriden GetHashCode method.
         */
        public override int GetHashCode()
        {
            return Reference.GetHashCode();
        }
    }
}

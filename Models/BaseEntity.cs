using System.ComponentModel.DataAnnotations;

namespace MerryClosets.Models
{
    public abstract class BaseEntity
    {
        [Key]
        public long Id { get; set; }
        public string Reference { get; set; }
        public long Version { get; set; }
        public bool IsActive { get; set; }

        public static bool ReferenceIsValid(string reference)
        {
            if (!string.IsNullOrEmpty(reference))
            {
                return true;
            }
            return false;
        }
    }
}

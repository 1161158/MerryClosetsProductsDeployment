using System.ComponentModel.DataAnnotations;

namespace MerryClosets.Models
{
    //[Owned]
    public abstract class Embedabble
    {
        [Key]
        public long Id { get; set; }
    }
}


using Newtonsoft.Json;

namespace MerryClosets.Models.DTO
{
    public class PriceDto : ValueObjectDto
    {
        public float Value { get; set; }

        public PriceDto(float value)
        {
            this.Value = value;
        }
        
        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}


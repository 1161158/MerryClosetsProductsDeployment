using Newtonsoft.Json;

namespace MerryClosets.Models.DTO
{
    public class ConfiguredMaterialDto : ValueObjectDto
    {
        public string OriginMaterialReference { get; set; }
        public string ColorReference { get; set; }
        public string FinishReference { get; set; }

        [JsonConstructor]
        public ConfiguredMaterialDto(string originMaterial, string color,
                    string finish)
        {
            this.OriginMaterialReference = originMaterial;
            this.ColorReference = color;
            this.FinishReference = finish;
        }
        protected ConfiguredMaterialDto() { }
        
        public override string ToString()
        {
            return this.OriginMaterialReference + " " + this.ColorReference + " " + this.FinishReference;
        }
    }
}
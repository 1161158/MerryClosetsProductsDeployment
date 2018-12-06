using Newtonsoft.Json;

namespace MerryClosets.Models.DTO
{
    public class ConfiguredDimensionDto : ValueObjectDto
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public int Depth { get; set; }

        [JsonConstructor]
        public ConfiguredDimensionDto(int height, int width, int depth)
        {
            this.Height = height;
            this.Width = width;
            this.Depth = depth;
        }

        public ConfiguredDimensionDto() { }
    }
}
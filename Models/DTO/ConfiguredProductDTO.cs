using System.Collections.Generic;
using MerryClosets.Models.DTO;
using Newtonsoft.Json;

namespace MerryClosets.Models.DTO
{
    public class ConfiguredProductDto : BaseEntityDto
    {
        public string ProductReference { get; set; }
        public string Description { get; set; }
        public ConfiguredMaterialDto ConfiguredMaterial { get; set; }
        public List<ConfiguredPartDto> Parts { get; set; }
        public ConfiguredDimensionDto ConfiguredDimension { get; set; }
        public List<ConfiguredSlotDto> ConfiguredSlots { get; set; }
        public PriceDto Price { get; set; }

        public ConfiguredProductDto(string productReference, string description, ConfiguredDimensionDto configuredDimension, ConfiguredMaterialDto material,
           List<ConfiguredPartDto> parts, string reference, List<ConfiguredSlotDto> configuredSlots)
        {
            this.ProductReference = productReference;
            this.Description = description;
            this.Reference = reference;
            this.Parts = parts;
            this.ConfiguredDimension = configuredDimension;
            this.ConfiguredMaterial = material;
            this.ConfiguredSlots = configuredSlots;
        }
        public ConfiguredProductDto(string productReference, ConfiguredDimensionDto configuredDimension, ConfiguredMaterialDto material,
            List<ConfiguredPartDto> parts, string reference, List<ConfiguredSlotDto> configuredSlots, PriceDto price)
        {
            this.ProductReference = productReference;
            this.Reference = reference;
            this.Parts = parts;
            this.ConfiguredDimension = configuredDimension;
            this.ConfiguredMaterial = material;
            this.ConfiguredSlots = configuredSlots;
            this.Price = price;
        }

        public ConfiguredProductDto() { }
    }
}
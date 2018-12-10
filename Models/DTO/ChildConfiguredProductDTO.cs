using System.Collections.Generic;
using MerryClosets.Models.DTO;
using Newtonsoft.Json;

namespace MerryClosets.Models.DTO
{
    public class ChildConfiguredProductDto : ConfiguredProductDto
    {
        public string ParentReference { get; set; }
        public string SlotReference { get; set; }

        [JsonConstructor]
        public ChildConfiguredProductDto(string parentReference, string slotReference, string productReference, ConfiguredDimensionDto configuredDimension, ConfiguredMaterialDto material, string reference, List<ConfiguredSlotDto> configuredSlots)
        {
            this.ParentReference = parentReference;
            this.SlotReference = slotReference;
            this.ProductReference = productReference;
            this.Reference = reference;
            this.ConfiguredDimension = configuredDimension;
            this.Parts = new List<ConfiguredPartDto>();
            this.ConfiguredMaterial = material;
            this.ConfiguredSlots = configuredSlots;
        }

        protected ChildConfiguredProductDto() { }

        /*
        [JsonConstructor] //for test purposes only
        public ConfiguredProductDto(long productId){
            this.ProductId = productId;
            Parts = new List<ConfiguredProductDto>();
        }*/
    }
}
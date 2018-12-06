using System.Collections.Generic;

namespace MerryClosets.Models.DTO
{
    public class ProductOrderDto : ConfiguredProductDto
    {
        public List<ConfiguredProductDto> ListProducts { get; set; }

        public ProductOrderDto(string productReference, ConfiguredDimensionDto configuredDimension,
            ConfiguredMaterialDto material, List<ConfiguredPartDto> parts, string reference,
            List<ConfiguredSlotDto> configuredSlots, List<ConfiguredProductDto> listProducts, PriceDto price) :
            base(productReference, configuredDimension, material, parts, reference, configuredSlots, price)
        {
            ListProducts = listProducts;
        }

        protected ProductOrderDto()
        {
            ListProducts = new List<ConfiguredProductDto>();
        }
    }
}
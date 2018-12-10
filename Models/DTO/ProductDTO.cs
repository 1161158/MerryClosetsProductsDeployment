using System.Collections.Generic;
using MerryClosets.Models.Material;
using Newtonsoft.Json;

namespace MerryClosets.Models.DTO
{
    public class ProductDto : BaseEntityDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string CategoryReference { get; set; }

        public PriceDto Price { get; set; }

        public List<PartDto> Parts { get; set; } = new List<PartDto>();

        public List<ProductMaterialDto> ProductMaterialList { get; set; } = new List<ProductMaterialDto>();

        public List<DimensionValuesDto> Dimensions { get; set; } = new List<DimensionValuesDto>();

        public SlotDefinitionDto SlotDefinition { get; set; }

        public ProductDto(string reference, string name, string description, string categoryReference, PriceDto price, List<PartDto> parts, List<ProductMaterialDto> productMaterialList, List<DimensionValuesDto> dimensions, SlotDefinitionDto slotDefinition)
        {
            this.Reference = reference;
            this.Name = name;
            this.Description = description;
            this.CategoryReference = categoryReference;
            this.Price = price;
            this.Parts = parts;
            this.ProductMaterialList = productMaterialList;
            this.Dimensions = dimensions;
            this.SlotDefinition = slotDefinition;
        }

        public ProductDto(string reference, string name, string description, string categoryReference, PriceDto price, SlotDefinitionDto slotDefinition)
        {
            this.Reference = reference;
            this.Name = name;
            this.Description = description;
            this.CategoryReference = categoryReference;
            this.Price = price;
            this.SlotDefinition = slotDefinition;
        }

        protected ProductDto() { }
    }
}
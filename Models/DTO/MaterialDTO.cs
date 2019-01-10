using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MerryClosets.Models.DTO
{
    public class MaterialDto : BaseEntityWithPriceHistoryDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public List<ColorDto> Colors { get; set; } = new List<ColorDto>();

        public List<FinishDto> Finishes { get; set; } = new List<FinishDto>();
        public string RelativeURL {get; set; }

        public MaterialDto(string reference, string name, string description, PriceDto price, string relativeURL)
        {
            this.Reference = reference;
            this.Description = description;
            this.Name = name;
            this.Price = price;
            this.RelativeURL = relativeURL;
        }

        public MaterialDto(string reference, string name, string description, PriceDto price, List<ColorDto> colors, List<FinishDto> finishes)
        {
            this.Reference = reference;
            this.Name = name;
            this.Description = description;
            this.Price = price;
            this.Colors = colors;
            this.Finishes = finishes;
        }

        protected MaterialDto() { }
/*
        public SimplifiedMaterialDto transform()
        {
            SimplifiedMaterialDto dto =
                new SimplifiedMaterialDto(this.Reference, this.Name, this.Description, this.Price);
            dto.Finishes = this.Finishes;
            List<MaterialColorDto> materialColors = new List<MaterialColorDto>();
            MaterialColorDto materialColor;
            foreach (var color in Colors)
            {
                materialColor = new MaterialColorDto();
                materialColor.ColorReference = color.Reference;
                materialColor.MaterialReference = this.Reference;
                materialColors.Add(materialColor);
            }

            dto.Colors = materialColors;
            dto.PriceHistory = PriceHistory;
            return dto;
        }*/
    }
}
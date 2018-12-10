using System.Collections.Generic;
using Newtonsoft.Json;

namespace MerryClosets.Models.DTO
{
    public class FinishDto : BaseEntityWithPriceHistoryDto
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public FinishDto(string name, string reference, string description, PriceDto price)
        {
            this.Name = name;
            this.Description = description;
            this.Reference = reference;
            this.Price = price;
        }

        protected FinishDto() { }
    }
}
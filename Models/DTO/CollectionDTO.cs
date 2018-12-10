using System.Collections.Generic;
using MerryClosets.Models.DTO;
using Newtonsoft.Json;

namespace MerryClosets.Models.DTO
{
    public class CollectionDto : BaseEntityDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public List<ProductCollectionDto> ProductCollectionList { get; set; } = new List<ProductCollectionDto>();

        public CollectionDto(string reference, string name, string description, List<ProductCollectionDto> productCollectionList)
        {
            this.Reference = reference;
            this.Name = name;
            this.Description = description;
            this.ProductCollectionList = productCollectionList;
        }

        public CollectionDto(string reference, string name, string description)
        {
            this.Reference = reference;
            this.Name = name;
            this.Description = description;
        }

        protected CollectionDto() { }
    }
}
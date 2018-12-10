using System.Collections.Generic;
using MerryClosets.Models.Collection;
using MerryClosets.Models.DTO;
using Newtonsoft.Json;

namespace MerryClosets.Models.DTO
{
    public class CatalogDto : BaseEntityDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public List<CatalogProductCollectionDto> CatalogProductCollectionList { get; set; } = new List<CatalogProductCollectionDto>();

        public CatalogDto(string reference, string name, string description, List<CatalogProductCollectionDto> catalogProductCollectionList)
        {
            this.Reference = reference;
            this.Name = name;
            this.Description = description;
            this.CatalogProductCollectionList = catalogProductCollectionList;
        }

        public CatalogDto(string reference, string name, string description)
        {
            this.Reference = reference;
            this.Name = name;
            this.Description = description;
        }

        protected CatalogDto() { }
    }
}
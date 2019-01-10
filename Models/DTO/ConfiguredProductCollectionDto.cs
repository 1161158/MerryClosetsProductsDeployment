using MerryClosets.Models.DTO;
using Newtonsoft.Json;

namespace MerryClosets.Models.DTO
{
    public class ConfiguredProductCollectionDto : EmbedabbleDto
    {
        public CollectionDto Collection { get; set; }
        public ConfiguredProductDto ConfiguredProduct { get; set; }

        public ConfiguredProductCollectionDto(ConfiguredProductDto configuredProduct, CollectionDto collection)
        {
            this.Collection = collection;
            this.ConfiguredProduct = configuredProduct;
        }
        protected ConfiguredProductCollectionDto(){}
        
        public override string ToString()
        {
            return this.Collection + " " + this.ConfiguredProduct;
        }
    }
}
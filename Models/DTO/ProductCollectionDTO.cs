using MerryClosets.Models.DTO;
using Newtonsoft.Json;

namespace MerryClosets.Models.DTO
{
    public class ProductCollectionDto : EmbedabbleDto
    {
        public string CollectionReference { get; set; }
        public string ConfiguredProductReference { get; set; }

        public ProductCollectionDto(string configuredProductReference, string collectionReference)
        {
            this.CollectionReference = collectionReference;
            this.ConfiguredProductReference = configuredProductReference;
        }
        protected ProductCollectionDto(){}
        
        public override string ToString()
        {
            return this.CollectionReference + " " + this.ConfiguredProductReference;
        }
    }
}
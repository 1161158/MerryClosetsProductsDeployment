using System;
using MerryClosets.Models.DTO;

namespace MerryClosets.Models.DTO
{
    public class CatalogProductCollectionDto : Embedabble
    {
        public string CatalogReference { get; set; }

        public ProductCollectionDto ProductCollection { get; set; }

        protected CatalogProductCollectionDto() { }

        public CatalogProductCollectionDto(string catalogReference, string configuredProductReference, string collectionReference)
        {
            this.CatalogReference = catalogReference;
            this.ProductCollection = new ProductCollectionDto(configuredProductReference, collectionReference);
        }

        public CatalogProductCollectionDto(string catalogReference, ProductCollectionDto productCollection)
        {
            this.CatalogReference = catalogReference;
            this.ProductCollection = productCollection;
        }
    }
}
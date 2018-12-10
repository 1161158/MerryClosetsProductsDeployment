using System;

namespace MerryClosets.Models.Collection
{
    public class CatalogProductCollection : Embedabble
    {
        public string CatalogReference { get; set; }

        public ProductCollection ProductCollection { get; set; }

        public CatalogProductCollection(string catalogReference, string configuredProductReference, string collectionReference)
        {
            this.CatalogReference = catalogReference;
            this.ProductCollection = new ProductCollection(configuredProductReference, collectionReference);
        }

        public CatalogProductCollection(string catalogReference, ProductCollection productCollection)
        {
            this.CatalogReference = catalogReference;
            this.ProductCollection = productCollection;
        }

        protected CatalogProductCollection() { }

        public override bool Equals(object obj)
        {
            if ((obj == null) || this.GetType() != obj.GetType())
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var pc = (CatalogProductCollection)obj;
            return string.Equals(this.CatalogReference, pc.CatalogReference, StringComparison.Ordinal) &&
                   this.ProductCollection.Equals(pc.ProductCollection);
        }

        public override int GetHashCode()
        {
            return Tuple.Create(CatalogReference, ProductCollection).GetHashCode();
        }
    }
}
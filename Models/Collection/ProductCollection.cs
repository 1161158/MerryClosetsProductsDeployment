using System;

namespace MerryClosets.Models.Collection
{
    public class ProductCollection : Embedabble
    {
        public string CollectionReference { get; set; }
        public string ConfiguredProductReference { get; set; }

        public ProductCollection(string configuredProductReference, string collectionReference)
        {
            this.ConfiguredProductReference = configuredProductReference;
            this.CollectionReference = collectionReference;
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || this.GetType() != obj.GetType())
            {
                return false;
            }

            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }

            ProductCollection pc = (ProductCollection)obj;
            return string.Equals(this.CollectionReference, pc.CollectionReference, StringComparison.Ordinal) &&
                   string.Equals(this.ConfiguredProductReference, pc.ConfiguredProductReference);
        }

        public override int GetHashCode()
        {
            return Tuple.Create(CollectionReference, ConfiguredProductReference).GetHashCode();
        }
    }
}
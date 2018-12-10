using System;
using System.Collections.Generic;

namespace MerryClosets.Models.Collection
{
    public class Collection : BaseEntity
    {
        /**
         * Name of the collection.
         */
        public string Name { get; set; }

        /**
         * Name of the description.
         */
        public string Description { get; set; }

        /**
         * List of configured products associated with a collection in the form os ProductCollection objects.
         */
        public List<ProductCollection> ProductCollectionList { get; set; } = new List<ProductCollection>();

        public Collection(string reference, string name, string description, List<ProductCollection> productCollectionList)
        {
            this.Reference = reference;
            this.Name = name;
            this.Description = description;
            this.ProductCollectionList = productCollectionList;
        }

        public Collection(string reference, string name, string description)
        {
            this.Reference = reference;
            this.Name = name;
            this.Description = description;
        }

        protected Collection() { }

        public void AddConfiguredProduct(string configuredProductReference)
        {
            this.ProductCollectionList.Add(new ProductCollection(configuredProductReference, this.Reference));
        }

        public void RemoveConfiguredProduct(string configuredProductReference)
        {
            this.ProductCollectionList.Remove(new ProductCollection(configuredProductReference, this.Reference));
        }

        /*
         * Method will verify if the configured product with the passed reference is associated with this collection.
         */
        public bool ConfiguredProductIsInCollection(string configuredProductReference)
        {
            return this.ProductCollectionList.Contains(new ProductCollection(configuredProductReference, this.Reference));
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

            Collection c = (Collection)obj;
            return string.Equals(Reference, c.Reference, StringComparison.Ordinal);
        }

        public override int GetHashCode()
        {
            return System.Tuple.Create(Reference).GetHashCode();
        }
    }
}
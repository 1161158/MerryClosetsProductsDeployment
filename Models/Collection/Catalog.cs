using System;
using System.Collections.Generic;

namespace MerryClosets.Models.Collection
{
    public class Catalog : BaseEntity
    {

        /**
         * Name of the catalog.
         */
        public string Name { get; set; }

        /**
         * Description of the catalog.
         */
        public string Description { get; set; }

        /**
         * List of all configured products in the catalog, each configured product is associated with a collection. This relation is captured in the form of CatalogProductCollection objects.
         */
        public List<CatalogProductCollection> CatalogProductCollectionList { get; set; } = new List<CatalogProductCollection>();

        public Catalog(string reference, string name, string description, List<CatalogProductCollection> catalogProductCollectionList)
        {
            this.Reference = reference;
            this.Name = name;
            this.Description = description;
            this.CatalogProductCollectionList = catalogProductCollectionList;
        }

        public Catalog(string reference, string name, string description)
        {
            this.Reference = reference;
            this.Name = name;
            this.Description = description;
        }

        protected Catalog() { }

        /**
         * 
         */
        public void AddProductCollection(ProductCollection newProductCollection)
        {
            this.CatalogProductCollectionList.Add(new CatalogProductCollection(this.Reference, newProductCollection));
        }

        /**
         * 
         */
        public void RemoveProductCollection(ProductCollection newProductCollection)
        {
            this.CatalogProductCollectionList.Remove(new CatalogProductCollection(this.Reference, newProductCollection));
        }

        /**
         * 
         */
        public bool ContainsProductCollection(ProductCollection prodCollection)
        {
            return this.CatalogProductCollectionList.Contains(new CatalogProductCollection(this.Reference, prodCollection));
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }

            Catalog other = (Catalog)obj;
            return string.Equals(this.Reference, other.Reference, StringComparison.Ordinal);
        }

        public override int GetHashCode()
        {
            return System.Tuple.Create(Reference).GetHashCode();
        }
    }
}
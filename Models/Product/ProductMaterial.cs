using System;
using System.Collections.Generic;
using MerryClosets.Models.Restriction;

namespace MerryClosets.Models.Product
{
    public class ProductMaterial : Embedabble
    {
        public string ProductReference { get; set; }

        public string MaterialReference { get; set; }

        public List<MaterialAlgorithm> Algorithms { get; set; } = new List<MaterialAlgorithm>();

        public ProductMaterial(string productReference, string materialRefer)
        {
            ProductReference = productReference;
            MaterialReference = materialRefer;
        }

        public ProductMaterial(string productReference, string materialRefer, List<MaterialAlgorithm> restrictions)
        {
            ProductReference = productReference;
            MaterialReference = materialRefer;
            Algorithms = restrictions;
        }

        protected ProductMaterial() { }

        public bool AddMaterialAlgorithm(MaterialAlgorithm materialAlg)
        {
            if (this.Algorithms.Contains(materialAlg))
            {
                return false;
            }
            Algorithms.Add(materialAlg);
            return true;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }

            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }

            var mp = obj as ProductMaterial;
            return string.Equals(this.MaterialReference, mp.MaterialReference, StringComparison.Ordinal);
        }

        public override int GetHashCode()
        {
            return MaterialReference.GetHashCode();
        }
    }
}
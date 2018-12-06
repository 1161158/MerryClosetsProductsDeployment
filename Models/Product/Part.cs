using System.Collections.Generic;
using MerryClosets.Models.Restriction;

namespace MerryClosets.Models.Product
{
    public class Part : Embedabble
    {
        public string ProductReference { get; set; }
        public List<PartAlgorithm> Algorithms { get; set; } = new List<PartAlgorithm>();

        public Part(string productReference)
        {
            ProductReference = productReference;
        }

        public Part(string productReference, List<PartAlgorithm> listPartAlgorithms)
        {
            ProductReference = productReference;
            Algorithms = listPartAlgorithms;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }

            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }

            var mp = obj as Part;
            return this.ProductReference.Equals(mp.ProductReference);
        }

        public override int GetHashCode()
        {
            return this.ProductReference.GetHashCode();
        }

        public bool AddPartAlgorithm(PartAlgorithm partAlgorithm){
            if(!this.Algorithms.Contains(partAlgorithm)){
                this.Algorithms.Add(partAlgorithm);
                return true;
            }
            return false;
        }
    }
}
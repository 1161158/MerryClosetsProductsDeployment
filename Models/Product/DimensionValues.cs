using System.Collections.Generic;
using MerryClosets.Models.Restriction;

namespace MerryClosets.Models.Product
{
    public class DimensionValues : ValueObject
    {
        public string Reference { get; set; }
        public List<Values> PossibleHeights { get; set; } = new List<Values>();
        public List<Values> PossibleWidths { get; set; } = new List<Values>();
        public List<Values> PossibleDepths { get; set; } = new List<Values>();
        public List<DimensionAlgorithm> Algorithms { get; set; } = new List<DimensionAlgorithm>();

        public DimensionValues(List<Values> possibleHeights, List<Values> possibleWidths, List<Values> possibleDepths, string Reference)
        {
            this.PossibleHeights = possibleHeights;
            this.PossibleWidths = possibleWidths;
            this.PossibleDepths = possibleDepths;
            this.Reference = Reference;
        }

        public DimensionValues(List<Values> possibleHeights, List<Values> possibleWidths, List<Values> possibleDepths,
            List<DimensionAlgorithm> restrictions, string Reference)
        {
            this.PossibleHeights = possibleHeights;
            this.PossibleWidths = possibleWidths;
            this.PossibleDepths = possibleDepths;
            this.Algorithms = restrictions;
            this.Reference = Reference;
        }

        protected DimensionValues()
        {
        }

        public bool CheckIfItBelongs(int width, int height, int depth)
        {
            foreach (var heightValues in PossibleHeights)
            {
                if (heightValues.IsValid(height))
                {
                    foreach (var widthValues in PossibleWidths)
                    {
                        if (widthValues.IsValid(width))
                        {
                            foreach (var depthValues in PossibleDepths)
                            {
                                if (depthValues.IsValid(depth))
                                {
                                    return true;
                                }
                            }

                            return false;
                        }
                    }

                    return false;
                }
            }

            return false;
        }


        public override int GetHashCode()
        {
            return this.PossibleHeights.GetHashCode() + this.PossibleWidths.GetHashCode() +
                   this.PossibleDepths.GetHashCode();
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

            var other = (DimensionValues)obj;
            return this.Reference == other.Reference;
            // return Compare.ContentOfTwoLists(this.PossibleHeights, other.PossibleHeights)
            //        && Compare.ContentOfTwoLists(this.PossibleWidths, other.PossibleWidths)
            //        && Compare.ContentOfTwoLists(this.PossibleDepths, other.PossibleDepths);
        }

        public bool AddDimensionAlgorithm(DimensionAlgorithm dimensionAlgorithm)
        {
            if (!this.Algorithms.Contains(dimensionAlgorithm))
            {
                this.Algorithms.Add(dimensionAlgorithm);
                return true;
            }
            return false;
        }
    }
}
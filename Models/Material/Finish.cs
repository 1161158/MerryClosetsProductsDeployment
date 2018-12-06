using System;

namespace MerryClosets.Models.Material
{
    public class Finish : BaseEntityWithPriceHistory
    {
        /**
         * Name of the finish.
         */
        public string Name { get; set; }

        /**
         * Description of the finish.
         */
        public string Description { get; set; }

        public Finish(string name, string reference, string description, Price price)
        {
            this.Name = name;
            this.Description = description;
            this.Reference = reference;
            this.Price = price;
        }

        protected Finish() { }

        
        public void RemovePriceHistory(PriceHistory priceHistory)
        {
            PriceHistory.Remove(priceHistory);
        }

        /* 
        * Compares the two objects. The 'Finish' objects are equal if the reference is the same.
        */
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

            var f = obj as Finish;
            return this.Reference.Equals(f.Reference);
        }

        public override int GetHashCode()
        {
            return System.Tuple.Create(this.Reference).GetHashCode();
        }

        public bool ChosenFinishIsValid(string reference)
        {
            return string.Equals(this.Reference, reference, StringComparison.Ordinal);
        }
    }
}
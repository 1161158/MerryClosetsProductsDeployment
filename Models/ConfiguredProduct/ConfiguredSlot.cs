using System;

namespace MerryClosets.Models.ConfiguredProduct
{
    public class ConfiguredSlot : Embedabble
    {
        public int Size { get; set; }
        public string Reference { get; set; }

        public ConfiguredSlot(int size, string reference)
        {
            this.Size = size;
            this.Reference = reference;
        }

        protected ConfiguredSlot()
        {
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

            var other = (ConfiguredSlot) obj;

            return this.Size == other.Size && string.Equals(this.Reference, other.Reference, StringComparison.Ordinal);
        }

        public override int GetHashCode()
        {
            return System.Tuple.Create(Size, Reference).GetHashCode();
        }
    }
}
using System;

namespace MerryClosets.Models.ConfiguredProduct
{
    public class ConfiguredPart : Embedabble
    {
        public string ConfiguredChildReference { get; set; }
        public string ChosenSlotReference { get; set; }

        public ConfiguredPart(string configuredChildReference, string chosenSlotReference)
        {
            this.ConfiguredChildReference = configuredChildReference;
            this.ChosenSlotReference = chosenSlotReference;
        }

        protected ConfiguredPart()
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

            var other = (ConfiguredPart) obj;
            return string.Equals(this.ChosenSlotReference, other.ChosenSlotReference, StringComparison.Ordinal) &&
                   string.Equals(this.ConfiguredChildReference, other.ConfiguredChildReference,
                       StringComparison.Ordinal);
        }

        public override int GetHashCode()
        {
            return System.Tuple.Create(ChosenSlotReference, ConfiguredChildReference).GetHashCode();
        }
    }
}
using System;
using System.Collections.Generic;

namespace MerryClosets.Models.ConfiguredProduct
{
    public class ConfiguredProduct : BaseEntity
    {
        public string ProductReference { get; set; }
        public ConfiguredMaterial ConfiguredMaterial { get; set; }
        public List<ConfiguredPart> Parts { get; set; } = new List<ConfiguredPart>();
        public ConfiguredDimension ConfiguredDimension { get; set; }
        public List<ConfiguredSlot> ConfiguredSlots { get; set; } = new List<ConfiguredSlot>();
        public Price Price { get; set; }

        public ConfiguredProduct(string productReference, string reference, ConfiguredMaterial material,
            ConfiguredDimension dimension, List<ConfiguredSlot> configuredSlots, Price price)
        {
            this.Reference = reference;
            this.ProductReference = productReference;
            this.ConfiguredDimension = dimension;
            this.ConfiguredSlots = configuredSlots;
            this.ConfiguredMaterial = material;
            this.Price = price;
        }

        public ConfiguredProduct(string productReference, string reference, ConfiguredMaterial material,
            ConfiguredDimension dimension, List<ConfiguredPart> parts, List<ConfiguredSlot> configuredSlots)
        {
            this.Reference = reference;
            this.ProductReference = productReference;
            this.Parts = parts;
            this.ConfiguredDimension = dimension;
            this.ConfiguredSlots = configuredSlots;
            this.ConfiguredMaterial = material;
        }

        protected ConfiguredProduct()
        {
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

            var cp = (ConfiguredProduct) obj;
            return string.Equals(this.Reference, cp.Reference, StringComparison.Ordinal);
        }

        /*public static bool ConfiguredDimensionDtoIsValid(ConfiguredDimensionDto dto)
        {
            return ConfiguredDimension.ConfiguredDimensionDtoIsValid(dto);
        }*/

        public override int GetHashCode()
        {
            return System.Tuple.Create(Reference).GetHashCode();
        }
    }
}
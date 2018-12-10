using System;

namespace MerryClosets.Models.Material
{
    public class Color : ValueObject
    {
        /**
         * Name of the color.
         */
        public string Name { get; set; }

        /**
         * Reference of the color, in HEXADECIMAL.
         */
        public string Description { get; set; }
        
        public string HexCode { get; set; }

        public Color(string name, string description, string hexCode)
        {
            this.Name = name;
            this.Description = description;
            this.HexCode = hexCode;
        }

        protected Color() { }

        /* 
        * Compares the two objects. The Colors are equal if the reference is the same.
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
            Color c = obj as Color;
            return this.HexCode.Equals(c.HexCode);
        }

        public override int GetHashCode()
        {
            return System.Tuple.Create(this.HexCode).GetHashCode();
        }
        
        public bool ChosenColorIsValid(string hex)
        {
            return string.Equals(this.HexCode, hex, StringComparison.Ordinal);
        }
    }
}
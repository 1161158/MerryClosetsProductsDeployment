using MerryClosets.Models.DTO;

namespace MerryClosets.Models.ConfiguredProduct
{
    public class ConfiguredDimension : ValueObject
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public int Depth { get; set; }

        public ConfiguredDimension(int height, int width, int depth)
        {
            this.Height = height;
            this.Width = width;
            this.Depth = depth;
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
                ConfiguredDimension d = (ConfiguredDimension)obj;
                return this.Height.Equals(d.Height) && this.Width.Equals(d.Width) && this.Depth.Equals(d.Depth);
            
        }

        public override int GetHashCode()
        {
            return System.Tuple.Create(Height, Width, Depth).GetHashCode();
        }

        public static bool ConfiguredDimensionDtoIsValid(ConfiguredDimensionDto dimension)
        {
            return dimension.Height > 0 && dimension.Width > 0 && dimension.Depth > 0;
        }
    }
}

namespace MerryClosets.Models.Product
{

    public class ContinuousValue : Values
    {
        public int MinValue { get; set; }
        public int MaxValue { get; set; }

        public ContinuousValue(int minValue, int maxValue)
        {
            this.MinValue = minValue;
            this.MaxValue = maxValue;
        }

        protected ContinuousValue() { }

        public override bool IsValid(int value)
        {
            return value <= MaxValue && value >= MinValue;
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

            var other = (ContinuousValue)obj;
            return this.MaxValue == other.MaxValue && this.MinValue == other.MinValue;
        }

        public override int GetHashCode()
        {
            return System.Tuple.Create(this.MaxValue, this.MinValue).GetHashCode();
        }

        public override string ToString()
        {
            return "MinValue: " + MinValue + " MaxValue" + MaxValue;
        }
    }
}
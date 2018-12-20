
namespace MerryClosets.Models.Product
{
    public class DiscreteValue : Values
    {
        public int Value { get; set; }

        public DiscreteValue(int value)
        {
            this.Value = value;
        }

        protected DiscreteValue() { }

        public override bool IsValid(int value)
        {
            return value == this.Value;
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

            var other = obj as DiscreteValue;
            return this.Value.CompareTo(other.Value) == 0;
        }

        public override int GetHashCode()
        {
            return System.Tuple.Create(Value).GetHashCode();
        }

        public override string ToString()
        {
            return "Value: " + this.Value.ToString();
        }
    }
}
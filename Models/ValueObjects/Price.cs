using MerryClosets.Models;
using MerryClosets.Models.DTO;

public class Price : ValueObject
{
    public float Value { get; set; }

    public Price(float value)
    {
        this.Value = value;
    }

    protected Price() { }

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

        var price = obj as Price;
        return this.Value == price.Value;
    }

    public override int GetHashCode()
    {
        return System.Tuple.Create(this.Value).GetHashCode();
    }
}

using MerryClosets.Models;

public class SlotDefinition : ValueObject
{
    public int MinSize { get; set; }
    public int MaxSize { get; set; }
    public int RecSize { get; set; }

    public SlotDefinition(int minSize, int maxSize, int recSize)
    {
        this.MinSize = minSize;
        this.MaxSize = maxSize;
        this.RecSize = recSize;
    }

    protected SlotDefinition() { }

}
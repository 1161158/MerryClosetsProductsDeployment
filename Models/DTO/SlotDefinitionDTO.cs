namespace MerryClosets.Models.DTO
{
    public class SlotDefinitionDto : ValueObjectDto
    {
        public int MinSize { get; set; }
        public int MaxSize { get; set; }
        public int RecSize { get; set; }

        public SlotDefinitionDto(int minSize, int maxSize, int recSize)
        {
            this.MinSize = minSize;
            this.MaxSize = maxSize;
            this.RecSize = recSize;
        }
        
        public override string ToString()
        {
            return this.MinSize.ToString() + " " + this.MaxSize.ToString() + " " + this.RecSize.ToString();
        }
    }
}
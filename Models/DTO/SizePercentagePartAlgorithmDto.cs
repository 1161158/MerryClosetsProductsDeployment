namespace MerryClosets.Models.DTO
{
    public class SizePercentagePartAlgorithmDto : PartAlgorithmDto
    {
        public string SizeType {get; set;}
        public int Min { get; set; }
        public int Max { get; set; }

        public SizePercentagePartAlgorithmDto(string sizeType, int min, int max) {
            this.SizeType = sizeType;
            this.Min = min;
            this.Max = max;
        }
        protected SizePercentagePartAlgorithmDto() {}
    }
}
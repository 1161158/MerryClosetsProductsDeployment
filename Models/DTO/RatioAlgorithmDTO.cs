namespace MerryClosets.Models.DTO
{
    public class RatioAlgorithmDto : DimensionAlgorithmDto
    {
        public string FirstValueDesc { get; set; }
        public string SecondValueDesc { get; set; }
        public string Operator { get; set; }
        public float Ratio { get; set; }

        public RatioAlgorithmDto(string firstValueDesc, string secondValueDesc, string op, float ratio)
        {
            this.FirstValueDesc = firstValueDesc;
            this.SecondValueDesc = secondValueDesc;
            this.Operator = op;
            this.Ratio = ratio;
        }
        protected RatioAlgorithmDto(){}
    }
}
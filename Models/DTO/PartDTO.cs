
using System.Collections.Generic;

namespace MerryClosets.Models.DTO
{
    public class PartDto : EmbedabbleDto
    {
        public string ProductReference { get; set; }
        public List<PartAlgorithmDto> Algorithms { get; set; } = new List<PartAlgorithmDto>();

        public PartDto(string productReference)
        {
            ProductReference = productReference;
        }

        public PartDto(string productReference, List<PartAlgorithmDto> listPartAlgorithms)
        {
            ProductReference = productReference;
            Algorithms = listPartAlgorithms;
        }

        protected PartDto() { }
        
        public override string ToString()
        {
            return this.ProductReference;
        }
    }
}


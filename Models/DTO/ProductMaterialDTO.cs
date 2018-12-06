using System.Collections.Generic;

namespace MerryClosets.Models.DTO
{
    public class ProductMaterialDto : EmbedabbleDto
    {
        public string ProductReference { get; set; }

        public string MaterialReference { get; set; }

        public List<MaterialAlgorithmDto> Algorithms { get; set; } = new List<MaterialAlgorithmDto>();

        public ProductMaterialDto(string productReference, string materialRefer)
        {
            ProductReference = productReference;
            MaterialReference = materialRefer;
        }

        public ProductMaterialDto(string productReference, string materialRefer, List<MaterialAlgorithmDto> restrictions)
        {
            ProductReference = productReference;
            MaterialReference = materialRefer;
            Algorithms = restrictions;
        }

        protected ProductMaterialDto() { }
        
        public override string ToString()
        {
            return this.ProductReference + " " + this.MaterialReference;
        }
    }
}
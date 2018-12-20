using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MerryClosets.Models.DTO
{
    public class DimensionValuesDto : ValueObjectDto
    {
        public string Reference { get; set; }
        public List<ValuesDto> PossibleHeights { get; set; }
        public List<ValuesDto> PossibleWidths { get; set; }
        public List<ValuesDto> PossibleDepths { get; set; }
        public List<DimensionAlgorithmDto> Algorithms { get; set; }

        public DimensionValuesDto(List<ValuesDto> possibleHeights, List<ValuesDto> possibleWidths, List<ValuesDto> possibleDepths, string reference)
        {
            this.PossibleHeights = possibleHeights;
            this.PossibleWidths = possibleWidths;
            this.PossibleDepths = possibleDepths;
            this.Algorithms = new List<DimensionAlgorithmDto>();
            this.Reference = reference;
        }

        public DimensionValuesDto(List<ValuesDto> possibleHeights, List<ValuesDto> possibleWidths, List<ValuesDto> possibleDepths, List<DimensionAlgorithmDto> restrictions, string reference)
        {
            this.PossibleHeights = possibleHeights;
            this.PossibleWidths = possibleWidths;
            this.PossibleDepths = possibleDepths;
            this.Algorithms = restrictions;
            this.Reference = reference;
        }

        protected DimensionValuesDto()
        {
            this.Algorithms = new List<DimensionAlgorithmDto>();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("{");
            if (PossibleHeights != null)
            {
                builder.Append("Possible Heights: ");
                builder.Append(PossibleHeights.ToString() + " ");
            }
            if (PossibleWidths != null)
            {
                builder.Append(" Possible Widths: ");
                builder.Append(PossibleHeights.ToString() + " ");
            }
            if (PossibleDepths != null)
            {
                builder.Append(" Possible Depths: ");
                builder.Append(PossibleHeights.ToString() + " ");
            }
            builder.Append("}");
            return builder.ToString();
        }
    }
}
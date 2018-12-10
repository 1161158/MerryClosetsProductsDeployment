using System.Collections.Generic;
using Newtonsoft.Json;

namespace MerryClosets.Models.DTO
{
    public class DimensionValuesDto : ValueObjectDto
    {
        public List<ValuesDto> PossibleHeights { get; set; }
        public List<ValuesDto> PossibleWidths { get; set; }
        public List<ValuesDto> PossibleDepths { get; set; }
        public List<DimensionAlgorithmDto> Algorithms { get; set; }

        public DimensionValuesDto(List<ValuesDto> possibleHeights, List<ValuesDto> possibleWidths, List<ValuesDto> possibleDepths)
        {
            this.PossibleHeights = possibleHeights;
            this.PossibleWidths = possibleWidths;
            this.PossibleDepths = possibleDepths;
            this.Algorithms = new List<DimensionAlgorithmDto>();
        }

        public DimensionValuesDto(List<ValuesDto> possibleHeights, List<ValuesDto> possibleWidths, List<ValuesDto> possibleDepths, List<DimensionAlgorithmDto> restrictions)
        {
            this.PossibleHeights = possibleHeights;
            this.PossibleWidths = possibleWidths;
            this.PossibleDepths = possibleDepths;
            this.Algorithms = restrictions;
        }

        protected DimensionValuesDto()
        {
            this.Algorithms = new List<DimensionAlgorithmDto>();
        }
        
        public override string ToString()
        {
            return PossibleHeights.ToString() + " " + PossibleWidths.ToString() + " " + PossibleDepths.ToString();
        }
    }
}
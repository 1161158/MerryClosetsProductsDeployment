using System.Collections.Generic;
using Newtonsoft.Json;

namespace MerryClosets.Models.DTO
{
    public class MaterialListDto
    {
        public List<MaterialDto> materialList { get; set; }

        [JsonConstructor]
        public MaterialListDto(List<MaterialDto> materialList)
        {
            this.materialList = materialList;
        }
    }
}
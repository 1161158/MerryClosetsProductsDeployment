using MerryClosets.Utils;

namespace MerryClosets.Models.DTO
{
    public class MaterialPartAlgorithmDto : PartAlgorithmDto
    {

        public MaterialPartAlgorithmDto(){
            //This algorithm requires no arguments
            base.type = Strings.MATERIAL_PART_ALGORITHM;
        }
    }
}
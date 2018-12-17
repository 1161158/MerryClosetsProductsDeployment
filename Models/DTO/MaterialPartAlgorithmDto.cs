using MerryClosets.Utils;

namespace MerryClosets.Models.DTO
{
    public class MaterialPartAlgorithmDto : PartAlgorithmDto
    {

        public MaterialPartAlgorithmDto(){
            //This algorithm requires no arguments
            base.type = RestrictionName.MATERIAL_PART_ALGORITHM;
        }
    }
}
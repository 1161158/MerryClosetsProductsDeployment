using MerryClosets.Utils;

namespace MerryClosets.Models.DTO
{
    public class MaterialFinishPartAlgorithmDto : PartAlgorithmDto
    {

        public MaterialFinishPartAlgorithmDto(){
            //This algorithm requires no arguments
            base.type = Strings.MATERIAL_FINISH_PART_ALGORITHM;
        }
    }
}
using MerryClosets.Utils;

namespace MerryClosets.Models.DTO
{
    public class SizePartAlgorithmDto : PartAlgorithmDto
    {

        public SizePartAlgorithmDto() {
            //This algorithm is NOT implemented yet
            base.type = Strings.SIZE_PART_ALGORITHM;
        }
    }
}
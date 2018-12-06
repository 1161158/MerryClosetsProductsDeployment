using System.Collections.Generic;

namespace MerryClosets.Models.DTO
{
    public abstract class BaseEntityWithPriceHistoryDto : BaseEntityDto
    {
        public List<PriceHistoryDto> PriceHistory { get; set; } = new List<PriceHistoryDto>();

        public PriceDto Price { get; set; }

        //https://www.youtube.com/watch?v=QEzhxP-pdos only the real ones will se this one
    }
}
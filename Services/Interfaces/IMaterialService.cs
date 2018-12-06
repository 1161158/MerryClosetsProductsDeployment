using System.Collections.Generic;
using MerryClosets.Models.DTO;
using MerryClosets.Models.Material;

namespace MerryClosets.Services.Interfaces
{
    public interface IMaterialService : IService<Material, MaterialDto>
    {
        ValidationOutput AddColorsToMaterial(string reference, IEnumerable<ColorDto> colorDtoEnumerable);
        ValidationOutput AddFinishesToMaterial(string reference, IEnumerable<FinishDto> finishDtoEnumerable);
        ValidationOutput RemoveColorsFromMaterial(string reference, IEnumerable<ColorDto> enumerableColorDto);
        ValidationOutput RemoveFinishesFromMaterial(string reference, IEnumerable<FinishDto> enumerableFinishDto);
        ValidationOutput AddPriceHistoryItemsToMaterial(string reference, IEnumerable<PriceHistoryDto> enumerablePriceHistoryDto);
        ValidationOutput AddPriceHistoryItemsToFinishOfMaterial(string materialReference, string finishReference, IEnumerable<PriceHistoryDto> enumerablePriceHistoryDto);
        ValidationOutput GetColors(string reference);
        ValidationOutput RemovePriceHistoryFromMaterial(string reference, IEnumerable<PriceHistoryDto> enumerablePriceHistoryDto);
        ValidationOutput DeleteFinishPriceHistoryFromMaterial(string reference, string finishReference, IEnumerable<PriceHistoryDto> enumerablePriceHistoryDto);
    }
}
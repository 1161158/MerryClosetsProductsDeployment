using System.Collections.Generic;
using MerryClosets.Models.ConfiguredProduct;
using MerryClosets.Models.DTO;

namespace MerryClosets.Services.Interfaces
{
    public interface IConfiguredProductService : IService<ConfiguredProduct, ConfiguredProductDto>
    {
        ValidationOutput GetAllInfoByReference(string reference);
        ValidationOutput GetAvailableProducts(string reference);
        IEnumerable<ConfiguredProductDto> GetAvailableToCollections();
    }
}
using System.Collections.Generic;
using MerryClosets.Models.Collection;
using MerryClosets.Models.DTO;

namespace MerryClosets.Services.Interfaces
{
    public interface ICollectionService : IService<Collection, CollectionDto>
    {
        ValidationOutput AddConfiguredProducts(string reference, IEnumerable<ConfiguredProductDto> enumerableConfiguredProduct);

        ValidationOutput DeleteConfiguredProducts(string reference, IEnumerable<ConfiguredProductDto> enumerableConfiguredProduct);
        
        ValidationOutput GetProductsCollection(string reference);
        
    }
}
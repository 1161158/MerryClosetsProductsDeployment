
using System.Collections.Generic;
using MerryClosets.Models.Collection;
using MerryClosets.Models.DTO;

namespace MerryClosets.Services.Interfaces
{
    public interface ICatalogService : IService<Catalog, CatalogDto>
    {
        ValidationOutput AddVariousProductCollection(string reference, IEnumerable<ProductCollectionDto> enumerableProductCollectionDto);
        ValidationOutput DeleteVariousProductCollection(string reference, IEnumerable<ProductCollectionDto> enumerableProductCollectionDto);
    }
}
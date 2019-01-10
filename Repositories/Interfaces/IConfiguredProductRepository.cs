using System.Collections.Generic;
using MerryClosets.Models.ConfiguredProduct;

namespace MerryClosets.Repositories.Interfaces
{
    public interface IConfiguredProductRepository : IRepository<ConfiguredProduct>
    {
        int ConfiguredProductsLenght();
        List<ConfiguredProduct> GetAvailablesToCollection();
    }
}
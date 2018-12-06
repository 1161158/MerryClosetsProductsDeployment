using MerryClosets.Models.ConfiguredProduct;

namespace MerryClosets.Repositories.Interfaces
{
    public interface IConfiguredProductRepository : IRepository<ConfiguredProduct>
    {
        int ConfiguredProductsLenght();
    }
}
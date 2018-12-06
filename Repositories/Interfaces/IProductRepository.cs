using System.Collections.Generic;
using MerryClosets.Models.Product;

namespace MerryClosets.Repositories.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        List<Product> ProductsOfCategory(string catRefer);
        List<Product> ProductsAndParts();
    }
}
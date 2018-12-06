using System.Collections.Generic;
using MerryClosets.Models.Category;

namespace MerryClosets.Repositories.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        List<Category> DirectChildCategories(string reference);
    }
}
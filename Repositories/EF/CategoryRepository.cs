using System.Collections.Generic;
using System.Linq;
using MerryClosets.Models.Category;
using MerryClosets.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Design;

namespace MerryClosets.Repositories.EF
{
    public class CategoryRepository : EfRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(MerryClosetsContext dbContext) : base(dbContext)
        {
        }

        public List<Category> DirectChildCategories(string reference)
        {
            return (from category in this.GetQueryable()
                where category.ParentCategoryReference != null && category.ParentCategoryReference.Equals(reference)
                select category).ToList();
            //return this.GetQueryable().Where(c => c.ParentCategoryReference != null && c.ParentCategoryReference.Equals(reference)).ToList();
        }
    }
}
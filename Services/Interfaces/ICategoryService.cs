using System.Collections.Generic;
using MerryClosets.Models.Category;
using MerryClosets.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace MerryClosets.Services.Interfaces
{
    public interface ICategoryService : IService<Category, CategoryDto>
    {
        ValidationOutput ObtainDirectChildCategories(string reference);
    }
}
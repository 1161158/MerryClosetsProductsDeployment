using MerryClosets.Models.Category;
using MerryClosets.Models.DTO;
using MerryClosets.Models.Product;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore;
using MerryClosets.Models.Collection;
using MerryClosets.Repositories.EF;
using MerryClosets.Repositories.Interfaces;

namespace MerryClosets.Repositories.EF
{
    public class CatalogRepository : EfRepository<Catalog>, ICatalogRepository
    {

        public CatalogRepository(MerryClosetsContext dbContext) : base(dbContext) { }

        public override Catalog GetByReference(string reference)
        {
            return this.GetQueryable()
            .Include(c => c.CatalogProductCollectionList).ThenInclude(d => d.ProductCollection)
            .FirstOrDefault(c => c.Reference == reference);
        }
    }
}
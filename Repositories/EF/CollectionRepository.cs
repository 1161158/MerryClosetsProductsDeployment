using System;
using System.Collections.Generic;
using System.Linq;
using MerryClosets.Models.Collection;
using MerryClosets.Repositories.EF;
using MerryClosets.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MerryClosets.Repositories
{

    public class CollectionRepository : EfRepository<Collection>, ICollectionRepository
    {

        public CollectionRepository(MerryClosetsContext context) : base(context) { }

        public override Collection GetByReference(string reference)
        {
            return this.GetQueryable()
            .Include(c => c.ProductCollectionList)
            .FirstOrDefault(c => c.Reference == reference);
        }
    }
}
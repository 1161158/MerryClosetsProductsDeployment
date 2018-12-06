using System.Collections.Generic;
using System.Linq;
using MerryClosets.Models.ConfiguredProduct;
using MerryClosets.Models.Product;
using MerryClosets.Repositories.EF;
using MerryClosets.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MerryClosets.Repositories.EF
{

    public class ConfiguredProductRepository : EfRepository<ConfiguredProduct>, IConfiguredProductRepository
    {

        public ConfiguredProductRepository(MerryClosetsContext dbContext) : base(dbContext)
        {
        }

        public override ConfiguredProduct GetById(long id) {
            var product = base.GetQueryable().Where(cp => cp.Id == id).Include(cp => cp.Parts).Include(cp => cp.ConfiguredMaterial).ThenInclude(cm => cm.ColorReference).Include(cp => cp.ConfiguredMaterial).ThenInclude(cm => cm.FinishReference).Include(cp => cp.ConfiguredDimension).FirstOrDefault();
            if(product == null){
                product = base.GetById(id);
            }
            return product;
        }

        public override ConfiguredProduct GetByReference(string reference){
            return this.GetQueryable()
            .Include(cp => cp.ConfiguredMaterial)
            .Include(cp => cp.Parts).Include(cp => cp.Price)
            .Include(cp => cp.ConfiguredDimension)
            .Include(cp => cp.ConfiguredSlots)
            .FirstOrDefault(cp => cp.Reference == reference);
        }

        public int ConfiguredProductsLenght()
        {
            List<ConfiguredProduct> configuredProducts = this.GetQueryable().ToList();
            return configuredProducts.Count;
        }

        // public ConfiguredProduct GetByProductId(long id){
        //     var list = base.GetQueryable();
        //     if (list != null)
        //     {
        //         return list.Where(cp => cp.Product == id).Include(cp => cp.Parts).Include(cp => cp.ConfiguredMaterial).Include(cp => cp.Dimension).FirstOrDefault();
        //     }

        //     return null;
        // }
    }
}
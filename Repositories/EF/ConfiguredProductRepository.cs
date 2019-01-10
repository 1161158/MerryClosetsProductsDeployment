using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
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

        public override ConfiguredProduct GetById(long id)
        {
            var product = base.GetQueryable().Where(cp => cp.Id == id).Include(cp => cp.Parts).Include(cp => cp.ConfiguredMaterial).ThenInclude(cm => cm.ColorReference).Include(cp => cp.ConfiguredMaterial).ThenInclude(cm => cm.FinishReference).Include(cp => cp.ConfiguredDimension).FirstOrDefault();
            if (product == null)
            {
                product = base.GetById(id);
            }
            return product;
        }

        public override ConfiguredProduct GetByReference(string reference)
        {
            return this.GetQueryable()
            .Include(cp => cp.ConfiguredMaterial)
            .Include(cp => cp.Parts)
            .Include(cp => cp.Price)
            .Include(cp => cp.ConfiguredDimension)
            .Include(cp => cp.ConfiguredSlots)
            .FirstOrDefault(cp => cp.Reference == reference);
        }

        public override List<ConfiguredProduct> List()
        {
            return base.GetActiveQueryable()
            .Include(p => p.Price)
            .Include(cp => cp.ConfiguredDimension).ToList();
        }

        public int ConfiguredProductsLenght()
        {
            List<ConfiguredProduct> configuredProducts = this.GetQueryable().ToList();
            return configuredProducts.Count;
        }

        public List<ConfiguredProduct> GetAvailablesToCollection()
        {
            var list = base.GetActiveQueryable()
                .Include(cp => cp.ConfiguredMaterial)
                .Include(cp => cp.Parts)
                .Include(cp => cp.ConfiguredSlots)
                .Include(p => p.Price)
                .Include(cp => cp.ConfiguredDimension)
                .Include(p => p.Parts).ToList();
            var configuredProductList = new List<ConfiguredProduct>();
            var exists = new List<bool>();
            foreach (var configuredProduct in list)
            {
                exists = new List<bool>();
                foreach (var parent in list)
                {
                    if (!parent.Contains(configuredProduct.Reference))
                    {
                        exists.Add(false);
                    }
                    else
                    {
                        exists.Add(true);
                    }
                }

                if (!exists.Contains(true))
                {
                    configuredProductList.Add(configuredProduct);
                }
                
            }

            return configuredProductList;
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
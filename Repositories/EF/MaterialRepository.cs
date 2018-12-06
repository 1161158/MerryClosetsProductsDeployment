using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MerryClosets.Models.Material;
using MerryClosets.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MerryClosets.Repositories.EF
{
    public class MaterialRepository : EfRepository<Material>, IMaterialRepository
    {
        
        public MaterialRepository(MerryClosetsContext dbContext) : base(dbContext) { }
        
        /**
         * The following snippet of code allows for the calculation of the current price of each finish in the passed material.
         */
        private void CalculateCurrentPriceEachFinish(Material material)
        {
            foreach (var finish in material.Finishes)
            {
                finish.Price = finish.CurrentPrice();
            }
        }

        /**
         * The following snippet of code allows for the calculation of the current price of the passed material.
         */
        private void CalculateCurrentPrice(Material material)
        {
            material.Price = material.CurrentPrice();
        }

        public override List<Material> List()
        {
            //List<Material> listToReturn = base.List();
            List<Material> listToReturn = this.GetActiveQueryable()
            .Include(m => m.Finishes).ThenInclude(n => n.PriceHistory).ThenInclude(n => n.Price)
            .Include(m => m.Colors)
            .Include(m => m.PriceHistory).ThenInclude(f => f.Price).ToList();

            foreach (var material in listToReturn)
            {
                CalculateCurrentPrice(material);
                CalculateCurrentPriceEachFinish(material);
            }

            return listToReturn;
        }

        public override Material GetByReference(string reference)
        {
            Material matToReturn = this.GetQueryable()
            .Include(m => m.Finishes).ThenInclude(n => n.PriceHistory).ThenInclude(n => n.Price)
            .Include(m => m.Colors)
            .Include(m => m.PriceHistory).ThenInclude(f => f.Price)
            .FirstOrDefault(m => m.Reference == reference);

            if (matToReturn != null)
            {
                CalculateCurrentPriceEachFinish(matToReturn);
                CalculateCurrentPrice(matToReturn);
            }

            return matToReturn;
        }
    }
}
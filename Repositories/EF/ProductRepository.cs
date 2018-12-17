using System.Collections.Generic;
using System.Linq;
using MerryClosets.Models.Product;
using MerryClosets.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MerryClosets.Repositories.EF
{
    public class ProductRepository : EfRepository<Product>, IProductRepository
    {
        public ProductRepository(MerryClosetsContext dbContext) : base(dbContext)
        {
        }

        public override Product GetByReference(string reference)
        {
            var product = base.GetQueryable().Where(p => p.Reference == reference)
            .Include(p => p.Parts).ThenInclude(part => part.Algorithms)
            .Include(p => p.SlotDefinition)
            .Include(p => p.ProductMaterialList)
            .Include(p => p.Dimensions).ThenInclude(sizes => sizes.PossibleWidths)
            .Include(p => p.Dimensions).ThenInclude(sizes => sizes.PossibleHeights)
            .Include(p => p.Dimensions).ThenInclude(sizes => sizes.PossibleDepths)
            .Include(p => p.Dimensions).ThenInclude(sizes => sizes.Algorithms)
            .Include(p => p.Price)
            .Include(p => p.ModelGroup).ThenInclude(modelGroup => modelGroup.Components).ThenInclude(component => component.Animation)
            .FirstOrDefault();

            if (product == null)
            {
                product = base.GetByReference(reference);
            }
            return product;
        }

        public List<Product> ProductsOfCategory(string catRefer)
        {
            return this.GetQueryable().Where(p => p.CategoryReference == catRefer).ToList();
        }
        public List<Product> ProductsAndParts(){
            return base.GetQueryable().Include(p => p.Parts).ThenInclude(part => part.Algorithms).ToList();
        }

        public override List<Product> List() {
            return base.GetActiveQueryable().Include(p => p.Price)
            .Include(p => p.SlotDefinition)
            .Include(p => p.ProductMaterialList).ToList();
        }
    }
}
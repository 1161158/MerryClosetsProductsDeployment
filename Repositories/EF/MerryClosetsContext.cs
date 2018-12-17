using Microsoft.EntityFrameworkCore;
using MerryClosets.Models.Product;
using MerryClosets.Models.Category;
using MerryClosets.Models.Material;
using MerryClosets.Models.Restriction;
using MerryClosets.Models.ConfiguredProduct;
using MerryClosets.Models.Collection;
using MerryClosets.Models.Animation;

namespace MerryClosets.Repositories.EF
{
    public class MerryClosetsContext : DbContext
    {


        public MerryClosetsContext(DbContextOptions<MerryClosetsContext> options)
            : base(options)
        { }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<RatioAlgorithm> RatioAlgorithms { get; set; }
        public DbSet<ConfiguredProduct> ConfiguredProducts { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<Catalog> Catalogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<DiscreteValue>();
        builder.Entity<ContinuousValue>();
        builder.Entity<MaterialFinishPartAlgorithm>();
        builder.Entity<MaterialPartAlgorithm>();
        builder.Entity<SizePercentagePartAlgorithm>();
        builder.Entity<SizePartAlgorithm>();
        builder.Entity<RatioAlgorithm>();
        builder.Entity<FrontalOpenAnimation>();
        builder.Entity<LateralOpenAnimation>();
        builder.Entity<SlidingLeftAnimation>();
        builder.Entity<SlidingRightAnimation>();


        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }

    }
}

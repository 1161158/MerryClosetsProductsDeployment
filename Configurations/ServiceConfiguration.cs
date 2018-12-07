using MerryClosets.Services.EF;
using MerryClosets.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace MerryClosets.Configurations
{
    public class ServiceConfiguration
    {
        public static void configure(IConfiguration configuration, IServiceCollection services)
        {
            useDefault(configuration, services);
        }

        private static void useDefault(IConfiguration configuration, IServiceCollection services)
        {
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<IMaterialService, MaterialService>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<ICatalogService, CatalogService>();
            services.AddTransient<ICollectionService, CollectionService>();
            services.AddTransient<IConfiguredProductService, ConfiguredProductService>();
            services.AddTransient<IUserValidationService, UserValidationService>();
        }
    }
}

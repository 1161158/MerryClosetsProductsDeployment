using MerryClosets.Repositories.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MerryClosets.Services.Interfaces;
using MerryClosets.Services.EF;
using MerryClosets.Repositories.Interfaces;
using MerryClosets.Repositories;
using MerryClosets.Models.DTO.DTOValidators;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MerryClosets.Configurations
{
    public enum DataProviderEnum
    {
        InMemory = 0,
        SQLite = 1,
        SQLServer = 2

    }
    public class DataConfiguration
    {
        public static void configure(DataProviderEnum provider, IConfiguration configuration, IServiceCollection services)
        {
            switch (provider)
            {
                case DataProviderEnum.InMemory:
                    addInMemory(configuration, services);
                    break;
                case DataProviderEnum.SQLite:
                    addSqlite(configuration, services);
                    break;
                case DataProviderEnum.SQLServer:
                    addSqlServer(configuration, services);
                    break;
            }
            useEntityFramework(configuration, services);
        }

        private static void addInMemory(IConfiguration configuration, IServiceCollection services)
        {
            services.AddDbContext<MerryClosetsContext>(options =>
                   options.UseInMemoryDatabase("MerryClosetsDB"));
        }

        private static void addSqlite(IConfiguration configuration, IServiceCollection services)
        {
            services.AddDbContext<MerryClosetsContext>(options =>
                   options.UseSqlite(configuration.GetConnectionString("MerryClosetsContext")));
        }

        private static void addSqlServer(IConfiguration configuration, IServiceCollection services)
        {
            services.AddDbContext<MerryClosetsContext>(options =>
                   options.UseSqlServer(configuration.GetConnectionString("MerryClosetsSQLSERVER")));
        }

        private static void useEntityFramework(IConfiguration configuration, IServiceCollection services)
        {
            // AddScoped(): These services are created once per request.
            // AddTransient(): These services are created each time they are requested.
            // AddSingleton(): These services are created first time they are requested and stay the same for subsequence requests.

            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<ICatalogRepository, CatalogRepository>();
            services.AddTransient<IMaterialRepository, MaterialRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<ICollectionRepository, CollectionRepository>();
            services.AddTransient<IConfiguredProductRepository, ConfiguredProductRepository>();
        }
    }
}

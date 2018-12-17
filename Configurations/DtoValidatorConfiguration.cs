using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MerryClosets.Models.DTO.DTOValidators;

namespace MerryClosets.Configurations
{
    public class DtoValidatorConfiguration
    {
        public static void configure(IConfiguration configuration, IServiceCollection services)
        {
            useDefault(configuration, services);
        }

        private static void useDefault(IConfiguration configuration, IServiceCollection services)
        {
            services.AddTransient<AlgorithmDTOValidator>();
            services.AddTransient<AnimationDTOValidator>();
            services.AddTransient<CategoryDTOValidator>();
            services.AddTransient<CatalogDTOValidator>();
            services.AddTransient<CollectionDTOValidator>();
            services.AddTransient<ColorDTOValidator>();
            services.AddTransient<ComponentDTOValidator>();
            services.AddTransient<ConfiguredProductDTOValidator>();
            services.AddTransient<ContinuousValueDTOValidator>();
            services.AddTransient<DimensionValuesDTOValidator>();
            services.AddTransient<DiscreteValueDTOValidator>();
            services.AddTransient<FinishDTOValidator>();
            services.AddTransient<MaterialDTOValidator>();
            services.AddTransient<ModelGroupDTOValidator>();
            services.AddTransient<PriceDTOValidator>();
            services.AddTransient<ProductDTOValidator>();
            services.AddTransient<RatioAlgorithmDTOValidator>();
            services.AddTransient<SlotDefinitionDTOValidator>();
            services.AddTransient<ValuesDTOValidator>();
            services.AddTransient<PriceHistoryDTOValidator>();
            services.AddTransient<SizePercentagePartAlgorithmDTOValidator>();
            services.AddTransient<RatioAlgorithmDTOValidator>();
        }
    }
}
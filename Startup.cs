using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using MerryClosets.Configurations;
using JsonSubTypes;
using MerryClosets.Models.DTO;
using MerryClosets.Models.Product;
using Newtonsoft.Json;
using MerryClosets.Models.Restriction;

namespace MerryClosets
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(JsonSubtypesConverterBuilder.Of(typeof(ValuesDto), "$type")
                .RegisterSubtype(typeof(DiscreteValueDto), ValuesDto.ValuesDtoType.DiscreteValueDto)
                .RegisterSubtype(typeof(ContinuousValueDto), ValuesDto.ValuesDtoType.ContinuousValueDto)
                .SerializeDiscriminatorProperty().Build());
            settings.Converters.Add(JsonSubtypesConverterBuilder.Of(typeof(Values), "$type")
                .RegisterSubtype(typeof(DiscreteValue), Values.ValuesType.DiscreteValue)
                .RegisterSubtype(typeof(ContinuousValue), Values.ValuesType.ContinuousValue)
                .SerializeDiscriminatorProperty().Build());
            settings.Converters.Add(JsonSubtypesConverterBuilder.Of(typeof(AlgorithmDto), "$type")
                .RegisterSubtype(typeof(MaterialFinishPartAlgorithmDto), AlgorithmDto.RestrictionDtoType.MaterialFinishPartAlgorithmDto)
                .RegisterSubtype(typeof(RatioAlgorithmDto), AlgorithmDto.RestrictionDtoType.RatioAlgorithmDto)
                .RegisterSubtype(typeof(SizePercentagePartAlgorithmDto), AlgorithmDto.RestrictionDtoType.SizePercentagePartAlgorithmDto)
                .SerializeDiscriminatorProperty().Build());
            settings.Converters.Add(JsonSubtypesConverterBuilder.Of(typeof(Algorithm), "$type")
                .RegisterSubtype(typeof(MaterialFinishPartAlgorithm), Algorithm.RestrictionType.MaterialFinishPartAlgorithm)
                .RegisterSubtype(typeof(RatioAlgorithm), Algorithm.RestrictionType.RatioAlgorithm)
                .RegisterSubtype(typeof(SizePercentagePartAlgorithm), Algorithm.RestrictionType.SizePercentagePartAlgorithm)
                .SerializeDiscriminatorProperty().Build());
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMvc(); //.SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddAutoMapper();

            int chosenDB = Configuration.GetValue("ChosenDB", 0);
            DataConfiguration.configure((DataProviderEnum)chosenDB, Configuration, services);
            ServiceConfiguration.configure(Configuration, services);

            services.AddMvc();
        }

        /*public void Configure(IApplicationBuilder app)
        {
            app.UseMvc();
        }*/
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder => builder.WithOrigins("https://merryclosetsstore.herokuapp.com").AllowAnyHeader().AllowAnyMethod());
            app.UseCors(builder => builder.WithOrigins("http://localhost:8000").AllowAnyHeader().AllowAnyMethod());
            app.UseMvc();
        }
    }
}
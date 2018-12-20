using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using MerryClosets.Configurations;

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
            
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMvc(); //.SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddAutoMapper();

            int chosenDB = Configuration.GetValue("ChosenDB", 0);
            DataConfiguration.configure((DataProviderEnum)chosenDB, Configuration, services);
            DtoValidatorConfiguration.configure(Configuration, services);
            ServiceConfiguration.configure(Configuration, services);
            AbstractClassConfiguration.configure();
            services.AddCors();
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

            app.UseCors(builder => builder.WithOrigins("*").AllowAnyHeader().AllowAnyMethod());
            app.UseCors(builder => builder.WithOrigins("*").AllowAnyHeader().AllowAnyMethod());
            app.UseMvc();
        }
    }
}
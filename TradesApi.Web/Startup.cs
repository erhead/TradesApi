using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Net.Http;
using TradesApi.BusinessLogic;
using TradesApi.Data;
using TradesApi.Data.EntityFramework;
using TradesApi.Data.InMemory;
using TradesApi.Data.Model;
using TradesApi.OpenExchangeRates;

namespace TradesApi.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Trades API", Version = "v1" });
            });

            services.AddHttpClient();

            services.AddDbContext<TradesApiContext>(options => options.UseSqlite(Configuration["DbConnectionString"]));

            services.AddScoped<IRepository<Currency>, CurrencyEfRepository>();
            services.AddScoped<IRepository<Trade>, TradesEfRepository>();
            services.AddSingleton<IConfigurationService>(new StaticConfigurationService
            {
                TotalEnrichmentPercent = decimal.Parse(Configuration["TotalEnrichmentPercent"])
            });
            services.AddScoped<ICurrencyRatesProvider>(sp =>
            {
                var factory = sp.GetRequiredService<IHttpClientFactory>();
                return new OxrCurrencyRatesProvider(factory.CreateClient(), Configuration["OxrAppId"])
                {
                    OxrApiRoot = Configuration["OxrApiRootUrl"]
                };
            });
            services.AddScoped<ITradesService, TradesService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Trades API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

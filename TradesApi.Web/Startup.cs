using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using TradesApi.BusinessLogic;
using TradesApi.Data;
using TradesApi.Data.InMemory;
using TradesApi.Data.Model;

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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Trades API", Version = "v1" });
            });

            //services.AddTransient<TradesApi.Data.ILogger>(sp => new Logger(sp.GetService<Microsoft.Extensions.Logging.ILogger>()));
            services.AddSingleton<ILogger, StubLogger>();
            services.AddSingleton<IRepository<Currency>>(new CurrencyInMemoryRepository());
            services.AddSingleton<IRepository<Trade>>(new TradesInMemoryRepository());
            services.AddSingleton<IConfigurationService>(new StaticConfigurationService());
            services.AddSingleton<ICurrencyRatesProvider>(new StaticCurrencyRatesProvider());
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

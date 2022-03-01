using EasyCaching.Core.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WageringRoulette.ApplicationServices;
using WageringRoulette.ApplicationServices.Abstraction;
using WageringRoulette.ApplicationServices.Configuration;
using WageringRoulette.DomainServices;
using WageringRoulette.DomainServices.Abstraction;
using WageringRoulette.Repositories;

namespace WageringRoulette
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
            services.AddMvc();
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "Ruleta de apuestas",
                    Description = "Servicios para la gestion de una ruleta de apuestas"
                });
            });

            services.AddEasyCaching(options =>
            {
                options.UseRedis(redisConfig =>
                {
                    redisConfig.DBConfig.Endpoints.Add(new ServerEndPoint("127.0.0.1", 6379));
                    redisConfig.DBConfig.AllowAdmin = true;
                }, "wageringRoulette");
            });

            services.Configure<AppConfiguration>(Configuration.GetSection("AppConfiguration"));

            services.AddScoped<IWageringRouletteRepository, WageringRouletteRepository>();
            services.AddScoped<IWageringRouletteDomainServie, WageringRouletteDomainServie>();
            services.AddScoped<IWageringRouletteApplicationServie, WageringRouletteApplicationServie>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mi Ruleta");
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

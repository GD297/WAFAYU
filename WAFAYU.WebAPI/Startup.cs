using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using WAFAYU.DataService.Extensions;
using WAFAYU.DataService.Models;
using WAFAYU.WebAPI.App_Start;
using WAFAYU.WebAPI.Handler;

namespace WAFAYU.WebAPI
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
            //services.AddDbContext<WAFAYUContext>();
            services.AddDbContext<wafayuContext>(options => options.UseSqlServer(Configuration.GetConnectionString("WAFAYUContext")));

            services.AddScoped<wafayuContext>();

            services.ConfigureDI();

            services.AddSwaggerGenNewtonsoftSupport();
            services.ConfigureSwagger();

            services.ConfigJwtBearer();

            services.ConfigureFilter<ErrorHandlingFilter>();

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.ConfigureSwagger(provider);
        }
    }
}

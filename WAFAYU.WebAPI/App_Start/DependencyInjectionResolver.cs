using Microsoft.Extensions.DependencyInjection;
using WAFAYU.DataService.DI;

namespace WAFAYU.WebAPI.App_Start
{
    public static class DependencyInjectionResolver
    {
        public static void ConfigureDI(this IServiceCollection services)
        {
            services.ConfigServicesDI();
            services.ConfigureAutoMapper();
        }
    }
}

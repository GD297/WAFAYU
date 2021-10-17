using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace WAFAYU.DataService.Extensions
{
    public static class FilterExtension
    {
        public static void ConfigureFilter<TErrorHandlingFilter>(this IServiceCollection services)
            where TErrorHandlingFilter : IExceptionFilter
        {
            services.AddMvc(ops =>
            {
            });
            services.AddControllers(options =>
            {
                options.Filters.Add<TErrorHandlingFilter>();
            });
        }
    }
}

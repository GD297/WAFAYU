using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using WAFAYU.DataService.AutoMapper;

namespace WAFAYU.WebAPI.App_Start
{
    public static class AutoMapperConfig
    {
        public static void ConfigureAutoMapper(this IServiceCollection services)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.ConfigStorageModule();
                mc.ConfigShelfModule();
                mc.ConfigOrderModule();
                mc.ConfigUserModule();
                mc.ConfigImageModule();
                mc.ConfigSpacePackageModule();
                mc.ConfigPendingOrderModule();
                mc.ConfigBoxModule();
                mc.ConfigFeedbackModule();
                mc.ConfigOrderDetailModule();
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
        }
    }
}

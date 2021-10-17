using AutoMapper;
using WAFAYU.DataService.Models;
using WAFAYU.DataService.ViewModels;
namespace WAFAYU.DataService.AutoMapper
{
    public static class PendingOrderModule
    {
        public static void ConfigPendingOrderModule(this IMapperConfigurationExpression mc)
        {
            mc.CreateMap<PendingOrderViewModel, PendingOrder>();
        }
    }
}

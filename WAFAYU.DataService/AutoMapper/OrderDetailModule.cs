using AutoMapper;
using WAFAYU.DataService.Models;
using WAFAYU.DataService.ViewModels;

namespace WAFAYU.DataService.AutoMapper
{
    public static class OrderDetailModule
    {
        public static void ConfigOrderDetailModule(this IMapperConfigurationExpression mc)
        {
            mc.CreateMap<OrderDetailViewModel, OrderDetail>()
                .ForMember(des => des.Status, opt => opt.MapFrom(src => 2));

            mc.CreateMap<OrderDetailUpdateViewModel, OrderDetail>();
        }
    }
}

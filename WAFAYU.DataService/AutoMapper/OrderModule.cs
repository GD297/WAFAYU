using AutoMapper;
using System;
using WAFAYU.DataService.Models;
using WAFAYU.DataService.ViewModels;

namespace WAFAYU.DataService.AutoMapper
{
    public static class OrderModule
    {
        public static void ConfigOrderModule(this IMapperConfigurationExpression mc)
        {
            mc.CreateMap<Order, CustomerOrderViewModel>()
                .ForMember(des => des.OwnerName, opt => opt.MapFrom(src => src.Owner.Name))
                .ForMember(dest => dest.ExpiredDate, opt => opt.MapFrom(src => src.PickupTime.Value.AddMonths((int)src.Months)))
                .ForMember(dest => dest.OwnerPhone, opt => opt.MapFrom(src => src.Owner.Phone));
            mc.CreateMap<CustomerOrderViewModel, Storage>();

            mc.CreateMap<Order, OwnerOrderViewModel>()
                .ForMember(dest => dest.ExpiredDate, opt => opt.MapFrom(src => src.PickupTime.Value.AddMonths((int)src.Months)));

            mc.CreateMap<Order, OwnerOrderDetailViewModel>()
                .ForMember(dest => dest.ExpiredDate, opt => opt.MapFrom(src => src.PickupTime.Value.AddMonths((int)src.Months)));

            mc.CreateMap<Order, CustomerOrderDetailViewModel>()
                .ForMember(dest => dest.ExpiredDate, opt => opt.MapFrom(src => src.PickupTime.Value.AddMonths((int)src.Months)));

            mc.CreateMap<OrderPaymentViewModel, Order>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => 1))
                .ForMember(dest => dest.PaidTime, opt => opt.MapFrom(src => DateTime.Now));
        }
    }
}

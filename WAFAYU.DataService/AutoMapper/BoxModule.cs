using AutoMapper;
using WAFAYU.DataService.Models;
using WAFAYU.DataService.ViewModels;

namespace WAFAYU.DataService.AutoMapper
{
    public static class BoxModule
    {
        public static void ConfigBoxModule(this IMapperConfigurationExpression mc)
        {
            mc.CreateMap<BoxViewModel, Box>();
            mc.CreateMap<Box, BoxViewModel>();

            mc.CreateMap<Box, BoxUsedViewModel>()
                .ForMember(dest => dest.BoxPosition, opt => opt.MapFrom(src => src.Position))
                .ForMember(dest => dest.BoxId, opt => opt.MapFrom(src => src.Id));
        }
    }
}

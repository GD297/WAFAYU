using AutoMapper;
using WAFAYU.DataService.AutoMapperConditions;
using WAFAYU.DataService.Models;
using WAFAYU.DataService.ViewModels;

namespace WAFAYU.DataService.AutoMapper
{
    public static class ShelfModule
    {
        public static void ConfigShelfModule(this IMapperConfigurationExpression mc)
        {
            mc.CreateMap<Shelf, ShelfViewModel>()
                .ForMember(dest => dest.Usage, opt => opt.MapFrom(src => ShelfAutoMapperPrecondition.CalculateBoxUsaged(src.Boxes)));
            mc.CreateMap<ShelfViewModel, Shelf>();

            mc.CreateMap<ShelfCreateViewModel, Shelf>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (1)));

            mc.CreateMap<Shelf, ShelfDetailViewModel>();
        }
    }
}

using AutoMapper;
using WAFAYU.DataService.Models;
using WAFAYU.DataService.ViewModels;

namespace WAFAYU.DataService.AutoMapper
{
    public static class ImageModule
    {
        public static void ConfigImageModule(this IMapperConfigurationExpression mc)
        {
            mc.CreateMap<Image, ImageViewModel>();
            mc.CreateMap<ImageViewModel, Image>();

            mc.CreateMap<Image, ImageUpdateViewModel>();
            mc.CreateMap<ImageUpdateViewModel, Image>();
        }
    }
}

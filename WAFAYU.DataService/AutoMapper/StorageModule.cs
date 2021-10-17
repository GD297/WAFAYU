using AutoMapper;
using WAFAYU.DataService.Models;
using WAFAYU.DataService.ViewModels;

namespace WAFAYU.DataService.AutoMapper
{
    public static class StorageModule
    {
        public static void ConfigStorageModule(this IMapperConfigurationExpression mc)
        {
            mc.CreateMap<Storage, StorageViewModel>()
                .ForMember(des => des.OwnerName, opt => opt.MapFrom(src => src.Owner.Name));
            //.ForMember(dest => dest.Picture, opt => opt.MapFrom(src => StorageAutoMapperPrecondition.GetStorageImgUrl(src.Picture)));
            mc.CreateMap<StorageViewModel, Storage>();

            mc.CreateMap<StorageCreateViewModel, Storage>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => 1));

            mc.CreateMap<StorageUpdateViewModel, Storage>();

            mc.CreateMap<Storage, StorageCreateSuccessViewModel>();

            mc.CreateMap<Storage, StorageDetailViewModel>();
        }
    }
}

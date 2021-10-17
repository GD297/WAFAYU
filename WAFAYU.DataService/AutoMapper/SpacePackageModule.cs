using AutoMapper;
using WAFAYU.DataService.Models;
using WAFAYU.DataService.ViewModels;

namespace WAFAYU.DataService.AutoMapper
{
    public static class SpacePackageModule
    {
        public static void ConfigSpacePackageModule(this IMapperConfigurationExpression mc)
        {
            mc.CreateMap<SpacePackageViewModel, SpacePackage>();
        }
    }
}

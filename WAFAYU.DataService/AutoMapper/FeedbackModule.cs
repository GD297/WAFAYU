using AutoMapper;
using WAFAYU.DataService.Models;
using WAFAYU.DataService.ViewModels;

namespace WAFAYU.DataService.AutoMapper
{
    public static class FeedbackModule
    {
        public static void ConfigFeedbackModule(this IMapperConfigurationExpression mc)
        {
            mc.CreateMap<Feedback, FeedbackViewModel>()
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.User.Avatar))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name));

            mc.CreateMap<FeedbackCreateViewModel, Feedback>();
            mc.CreateMap<Feedback, FeedbackCreateViewModel>();
        }
    }
}

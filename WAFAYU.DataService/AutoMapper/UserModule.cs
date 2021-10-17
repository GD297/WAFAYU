using AutoMapper;
using Newtonsoft.Json.Linq;
using WAFAYU.DataService.Models;
using WAFAYU.DataService.ViewModels;

namespace WAFAYU.DataService.AutoMapper
{
    public static class UserModule
    {
        public static void ConfigUserModule(this IMapperConfigurationExpression mc)
        {
            mc.CreateMap<ChangePasswordViewModel, FirebaseChangePasswordModel>()
                .ForMember(des => des.password, opt => opt.MapFrom(src => src.newPassword))
                .ForMember(des => des.returnSecureToken, opt => opt.MapFrom(src => true));

            mc.CreateMap<LoginViewModel, FirebaseLoginViewModel>();
            //mc.CreateMap<LoginViewModel, FirebaseLoginViewModel>()
            //    .ForMember(des => des.returnSecureToken, opt => opt.MapFrom(src => true));
            mc.CreateMap<JObject, TokenViewModel>()
                .ForMember(des => des.idToken, cfg => { cfg.MapFrom(jo => jo["idToken"]); })
                .ForMember(des => des.expiresIn, cfg => { cfg.MapFrom(jo => jo["expiresIn"]); })
                .ForMember(des => des.localId, cfg => { cfg.MapFrom(jo => jo["localId"]); })
                .ForMember(des => des.refreshToken, cfg => { cfg.MapFrom(jo => jo["refreshToken"]); });

            mc.CreateMap<SignUpViewModel, FirebaseSignUpModel>()
                .ForMember(des => des.email, opt => opt.MapFrom(src => src.Email))
                .ForMember(des => des.password, opt => opt.MapFrom(src => src.Password))
                .ForMember(des => des.returnSecureToken, opt => opt.MapFrom(src => true));
            mc.CreateMap<SignUpViewModel, User>();

            mc.CreateMap<UpdateProfileViewModel, User>();
            mc.CreateMap<User, UpdateProfileViewModel>();

            mc.CreateMap<User, TokenViewModel>()
                .ForMember(des => des.displayName, opt => opt.MapFrom(src => src.Name));

            mc.CreateMap<TokenModel,TokenViewModel>();

            mc.CreateMap<FirebaseUserViewModel, TokenViewModel>();
        }
    }
}

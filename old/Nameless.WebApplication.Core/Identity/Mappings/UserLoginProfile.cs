using System.Data;
using AutoMapper;
using Nameless.Data;

namespace Nameless.WebApplication.Identity {
    public class UserLoginProfile : Profile {
        #region Public Constructors

        public UserLoginProfile () {
            CreateMap<IDataRecord, UserLogin> ()
                .ForMember (dest => dest.UserID, opts => opts.MapFrom (src => src.GetGuidOrDefault (nameof (UserLogin.UserID), null).GetValueOrDefault ()))
                .ForMember (dest => dest.LoginProvider, opts => opts.MapFrom (src => src.GetStringOrDefault (nameof (UserLogin.LoginProvider), null)))
                .ForMember (dest => dest.ProviderKey, opts => opts.MapFrom (src => src.GetStringOrDefault (nameof (UserLogin.ProviderKey), null)))
                .ForMember (dest => dest.ProviderDisplayName, opts => opts.MapFrom (src => src.GetStringOrDefault (nameof (UserLogin.ProviderDisplayName), null)));
        }

        #endregion
    }
}
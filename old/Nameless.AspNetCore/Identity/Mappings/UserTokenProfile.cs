using System.Data;
using AutoMapper;
using Nameless.Data;

namespace Nameless.AspNetCore.Identity {
    public class UserTokenProfile : Profile {
        #region Public Constructors

        public UserTokenProfile () {
            CreateMap<IDataRecord, UserToken> ()
                .ForMember (dest => dest.UserID, opts => opts.MapFrom (src => src.GetGuidOrDefault (nameof (UserToken.UserID), null).GetValueOrDefault ()))
                .ForMember (dest => dest.LoginProvider, opts => opts.MapFrom (src => src.GetStringOrDefault (nameof (UserToken.LoginProvider), null)))
                .ForMember (dest => dest.Name, opts => opts.MapFrom (src => src.GetStringOrDefault (nameof (UserToken.Name), null)))
                .ForMember (dest => dest.Value, opts => opts.MapFrom (src => src.GetStringOrDefault (nameof (UserToken.Value), null)));
        }

        #endregion
    }
}
using System.Data;
using AutoMapper;
using Nameless.Data;

namespace Nameless.WebApplication.Identity {
    public class UserClaimProfile : Profile {
        #region Public Constructors

        public UserClaimProfile () {
            CreateMap<IDataRecord, UserClaim> ()
                .ForMember (dest => dest.UserID, opts => opts.MapFrom (src => src.GetGuidOrDefault (nameof (UserClaim.UserID), null).GetValueOrDefault ()))
                .ForMember (dest => dest.Type, opts => opts.MapFrom (src => src.GetStringOrDefault (nameof (UserClaim.Type), null)))
                .ForMember (dest => dest.Value, opts => opts.MapFrom (src => src.GetStringOrDefault (nameof (UserClaim.Value), null)));
        }

        #endregion
    }
}
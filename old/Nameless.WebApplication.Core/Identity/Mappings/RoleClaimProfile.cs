using System.Data;
using AutoMapper;
using Nameless.Data;

namespace Nameless.WebApplication.Identity {
    public class RoleClaimProfile : Profile {
        #region Public Constructors

        public RoleClaimProfile () {
            CreateMap<IDataRecord, RoleClaim> ()
                .ForMember (dest => dest.RoleID, opts => opts.MapFrom (src => src.GetGuidOrDefault (nameof (RoleClaim.RoleID), null).GetValueOrDefault ()))
                .ForMember (dest => dest.Type, opts => opts.MapFrom (src => src.GetStringOrDefault (nameof (RoleClaim.Type), null)))
                .ForMember (dest => dest.Value, opts => opts.MapFrom (src => src.GetStringOrDefault (nameof (RoleClaim.Value), null)));
        }

        #endregion
    }
}
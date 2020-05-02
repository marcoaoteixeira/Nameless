using System.Data;
using AutoMapper;
using Nameless.Data;

namespace Nameless.AspNetCore.Identity {
    public class RoleProfile : Profile {
        #region Public Constructors

        public RoleProfile () {
            CreateMap<IDataRecord, Role> ()
                .ForMember (dest => dest.ID, opts => opts.MapFrom (src => src.GetGuidOrDefault (nameof (Role.ID), null).GetValueOrDefault ()))
                .ForMember (dest => dest.Name, opts => opts.MapFrom (src => src.GetStringOrDefault (nameof (Role.Name), null)))
                .ForMember (dest => dest.NormalizedName, opts => opts.MapFrom (src => src.GetStringOrDefault (nameof (Role.NormalizedName), null)));
        }

        #endregion
    }
}
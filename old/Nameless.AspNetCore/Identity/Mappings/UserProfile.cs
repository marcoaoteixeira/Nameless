using System.Data;
using AutoMapper;
using Nameless.Data;

namespace Nameless.AspNetCore.Identity {
    public class UserProfile : Profile {
        #region Public Constructors

        public UserProfile () {
            CreateMap<IDataRecord, User> ()
                .ForMember (dest => dest.ID, opts => opts.MapFrom (src => src.GetGuidOrDefault (nameof (User.ID), null).GetValueOrDefault ()))
                .ForMember (dest => dest.Email, opts => opts.MapFrom (src => src.GetStringOrDefault (nameof (User.Email), null)))
                .ForMember (dest => dest.EmailConfirmed, opts => opts.MapFrom (src => src.GetBooleanOrDefault (nameof (User.EmailConfirmed), null).GetValueOrDefault ()))
                .ForMember (dest => dest.NormalizedEmail, opts => opts.MapFrom (src => src.GetStringOrDefault (nameof (User.NormalizedEmail), null)))
                .ForMember (dest => dest.UserName, opts => opts.MapFrom (src => src.GetStringOrDefault (nameof (User.UserName), null)))
                .ForMember (dest => dest.NormalizedUserName, opts => opts.MapFrom (src => src.GetStringOrDefault (nameof (User.NormalizedUserName), null)))
                .ForMember (dest => dest.PhoneNumber, opts => opts.MapFrom (src => src.GetStringOrDefault (nameof (User.PhoneNumber), null)))
                .ForMember (dest => dest.PhoneNumberConfirmed, opts => opts.MapFrom (src => src.GetBooleanOrDefault (nameof (User.PhoneNumberConfirmed), null).GetValueOrDefault ()))
                .ForMember (dest => dest.LockoutEnabled, opts => opts.MapFrom (src => src.GetBooleanOrDefault (nameof (User.LockoutEnabled), null).GetValueOrDefault ()))
                .ForMember (dest => dest.LockoutEnd, opts => opts.MapFrom (src => src.GetDateTimeOffsetOrDefault (nameof (User.LockoutEnd), null)))
                .ForMember (dest => dest.AccessFailedCount, opts => opts.MapFrom (src => src.GetInt32OrDefault (nameof (User.AccessFailedCount), null).GetValueOrDefault ()))
                .ForMember (dest => dest.PasswordHash, opts => opts.MapFrom (src => src.GetStringOrDefault (nameof (User.PasswordHash), null)))
                .ForMember (dest => dest.SecurityStamp, opts => opts.MapFrom (src => src.GetStringOrDefault (nameof (User.SecurityStamp), null)))
                .ForMember (dest => dest.TwoFactorEnabled, opts => opts.MapFrom (src => src.GetBooleanOrDefault (nameof (User.TwoFactorEnabled), null).GetValueOrDefault ()))
                .ForMember (dest => dest.AvatarUrl, opts => opts.MapFrom (src => src.GetStringOrDefault (nameof (User.AvatarUrl), null)));
        }

        #endregion
    }
}
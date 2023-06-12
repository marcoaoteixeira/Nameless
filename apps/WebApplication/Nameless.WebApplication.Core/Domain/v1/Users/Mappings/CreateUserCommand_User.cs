using AutoMapper;
using Nameless.WebApplication.Domain.v1.Users.Commands;
using Nameless.WebApplication.Entities;

namespace Nameless.WebApplication.Domain.v1.Users.Mappings {

    public sealed class CreateUserCommand_User : Profile {

        #region Public Constructors

        public CreateUserCommand_User() {
            CreateMap<CreateUserCommand, User>()
                .ForMember(dest => dest.Id, opts => opts.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.UserName, opts => opts.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opts => opts.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opts => opts.MapFrom(src => src.PhoneNumber));
        }

        #endregion
    }
}

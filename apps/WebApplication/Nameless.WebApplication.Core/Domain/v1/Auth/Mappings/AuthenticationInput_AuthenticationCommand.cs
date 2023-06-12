using AutoMapper;
using Nameless.WebApplication.Domain.v1.Auth.Commands;
using Nameless.WebApplication.Domain.v1.Auth.Models.Input;

namespace Nameless.WebApplication.Domain.v1.Auth.Mappings {

    public sealed class AuthenticationInput_AuthenticationCommand : Profile {

        #region Public Constructors

        public AuthenticationInput_AuthenticationCommand() {
            CreateMap<AuthenticationInput, AuthenticationCommand>();
        }

        #endregion
    }
}

using AutoMapper;
using Nameless.WebApplication.Domain.v1.Token.Commands;
using Nameless.WebApplication.Domain.v1.Token.Models.Input;

namespace Nameless.WebApplication.Domain.v1.Token.Mappings {

    public sealed class TokenInput_RefreshTokenCommand : Profile {

        #region Public Constructors

        public TokenInput_RefreshTokenCommand() {
            CreateMap<TokenInput, RefreshTokenCommand>();
        }

        #endregion
    }
}

using MediatR;
using Nameless.Web.Identity.Api.Responses;

namespace Nameless.Web.Identity.Api.Requests {
    public sealed record AuthenticateUserRequest : IRequest<AuthenticateUserResponse> {
        #region Public Properties

        public string UserName { get; init; } = string.Empty;
        
        public string Password { get; init; } = string.Empty;

        #endregion
    }
}

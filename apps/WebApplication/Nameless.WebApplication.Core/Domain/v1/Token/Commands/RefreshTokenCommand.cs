using System.Security.Claims;
using AutoMapper;
using FluentValidation;
using Nameless.CommandQuery;
using Nameless.Infrastructure;
using Nameless.Logging;
using Nameless.WebApplication.Domain.v1.Token.Models.Output;
using Nameless.WebApplication.Services;

namespace Nameless.WebApplication.Domain.v1.Token.Commands {

    public sealed class RefreshTokenCommand : ICommand {

        #region Public Properties

        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;

        #endregion
    }

    public sealed class RefreshTokenCommandHandler : CommandHandlerBase<RefreshTokenCommand> {

        #region Private Read-Only Fields

        private readonly IAccessTokenService _accessTokenService;
        private readonly IRefreshTokenService _refreshTokenService;

        #endregion

        #region Public Constructors

        public RefreshTokenCommandHandler(
            IAccessTokenService accessTokenService,
            IRefreshTokenService refreshTokenService,
            IMapper mapper,
            IValidator<RefreshTokenCommand>? validator = null
        )
            : base(mapper, validator) {
            Prevent.Null(accessTokenService, nameof(accessTokenService));
            Prevent.Null(refreshTokenService, nameof(refreshTokenService));

            _accessTokenService = accessTokenService;
            _refreshTokenService = refreshTokenService;
        }

        #endregion

        #region Protected Override Methods

        protected override async Task<ExecutionResult> InnerHandleAsync(RefreshTokenCommand command, CancellationToken cancellationToken = default) {
            try {
                var principal = await _accessTokenService.ExtractAsync(command.AccessToken, cancellationToken);
                var userId = principal.Claims.Single(_ => _.Type == ClaimTypes.NameIdentifier).Value;
                var userName = principal.Claims.Single(_ => _.Type == ClaimTypes.Name).Value;
                var userEmail = principal.Claims.Single(_ => _.Type == ClaimTypes.Email).Value;

                var accessToken = await _accessTokenService.GenerateAsync(Guid.Parse(userId), userName, userEmail, cancellationToken);
                var refreshToken = await _refreshTokenService.ReplaceAsync(command.RefreshToken, cancellationToken);

                return ExecutionResult.Successful(new TokenOutput {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                });
            } catch (Exception ex) {
                Logger.Error(ex, ex.Message);
                return ExecutionResult.Failure(ex.Message);
            }
        }

        #endregion
    }
}

using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Nameless.CommandQuery;
using Nameless.Infrastructure;
using Nameless.WebApplication.Domain.v1.Auth.Models.Output;
using Nameless.WebApplication.Entities;
using Nameless.WebApplication.Services;

namespace Nameless.WebApplication.Domain.v1.Auth.Commands {

    public sealed class AuthenticationCommand : ICommand {

        #region Public Properties

        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;

        #endregion
    }

    public sealed class AuthenticationCommandHandler : CommandHandlerBase<AuthenticationCommand> {

        #region Private Read-Only Fields

        private readonly SignInManager<User> _signInManager;
        private readonly IAccessTokenService _accessTokenService;
        private readonly IRefreshTokenService _refreshTokenService;

        #endregion

        #region Public Constructors

        public AuthenticationCommandHandler(
            SignInManager<User> signInManager,
            IAccessTokenService accessTokenService,
            IRefreshTokenService refreshTokenService,
            IMapper mapper,
            IValidator<AuthenticationCommand> validator
        ) : base(mapper, validator) {
            Prevent.Null(signInManager, nameof(signInManager));
            Prevent.Null(accessTokenService, nameof(accessTokenService));
            Prevent.Null(refreshTokenService, nameof(refreshTokenService));

            _signInManager = signInManager;
            _accessTokenService = accessTokenService;
            _refreshTokenService = refreshTokenService;
        }

        #endregion

        #region Protected Override Methods

        protected override async Task<ExecutionResult> InnerHandleAsync(AuthenticationCommand command, CancellationToken cancellationToken = default) {
            var user = await _signInManager.UserManager.FindByEmailAsync(command.Email);
            if (user == default) { return ExecutionResult.Failure("User not found."); }

            var result = await _signInManager.PasswordSignInAsync(user.UserName!, command.Password, isPersistent: false, lockoutOnFailure: true);
            if (result.IsLockedOut) { return ExecutionResult.Failure("User locked out."); }
            if (!result.Succeeded) { return ExecutionResult.Failure("User name or password incorrect."); }

            var accessToken = await _accessTokenService.GenerateAsync(user.Id, user.UserName!, user.Email!, cancellationToken);
            var refreshToken = await _refreshTokenService.GenerateAsync(user.Id, cancellationToken);

            var claims = await _signInManager
                .UserManager
                .GetClaimsAsync(user);

            var state = new AuthenticationOutput {
                UserId = user.Id.ToString(),
                UserName = user.UserName!,
                Email = user.Email!,
                Claims = claims.ToDictionary(
                    keySelector: key => key.Type,
                    elementSelector: element => claims
                        .Where(claim => claim.Type == element.Type)
                        .Select(claim => claim.Value)
                        .ToArray()
                ),
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            return ExecutionResult.Successful(state);
        }

        #endregion
    }
}

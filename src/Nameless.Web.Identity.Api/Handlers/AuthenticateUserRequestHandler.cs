using MediatR;
using Microsoft.AspNetCore.Identity;
using Nameless.Web.Identity.Api.Requests;
using Nameless.Web.Identity.Api.Responses;
using Nameless.Web.Services;

namespace Nameless.Web.Identity.Api.Handlers;

public sealed class AuthenticateUserRequestHandler<TUser, TKey> : IRequestHandler<AuthenticateUserRequest, AuthenticateUserResponse>
    where TUser : IdentityUser<TKey>
    where TKey : IEquatable<TKey> {
    private readonly SignInManager<TUser> _signInManager;
    private readonly IJwtService _jwtService;

    public AuthenticateUserRequestHandler(SignInManager<TUser> signInManager, IJwtService jwtService) {
        _signInManager = Prevent.Argument.Null(signInManager);
        _jwtService = Prevent.Argument.Null(jwtService);
    }

    public async Task<AuthenticateUserResponse> Handle(AuthenticateUserRequest request, CancellationToken cancellationToken) {
        var result = await _signInManager.PasswordSignInAsync(request.UserName,
                                                              request.Password,
                                                              isPersistent: false,
                                                              lockoutOnFailure: false);

        if (!result.Succeeded) {
            return AuthenticateUserResponse.InvalidCredentials;
        }

        if (result.IsNotAllowed) {
            return AuthenticateUserResponse.UserNotAllowed;
        }

        if (result.IsLockedOut) {
            return AuthenticateUserResponse.UserLockedOut;
        }

        if (result.RequiresTwoFactor) {
            return AuthenticateUserResponse.UserRequiresTwoFactorAuth;
        }

        var user = await _signInManager.UserManager
                                       .FindByEmailAsync(request.UserName);

        if (user is null) {
            return AuthenticateUserResponse.UserNotFound;
        }

        var token = _jwtService.Generate(new JwtParameters {
            Sub = user.Id.ToString(),
            Name = user.UserName,
            Email = user.Email,
        });

        return new AuthenticateUserResponse {
            Token = token
        };
    }
}
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nameless.Mediator;
using Nameless.Web.Endpoints;
using Nameless.Web.Endpoints.Definitions;
using Nameless.Web.Identity.Domains.UserRefreshTokens;
using Nameless.Web.Identity.UseCases.SecurityTokens.AccessToken;
using Nameless.Web.Identity.UseCases.SecurityTokens.RefreshTokens;

using static Microsoft.AspNetCore.Http.TypedResults;

namespace Nameless.Web.Identity.Endpoints.v1.Auth.Refresh;

public record RefreshInput {
    public required string Token { get; init; }
}

public record RefreshOutput {
    public required string AccessToken { get; init; }

    public string? RefreshToken { get; init; }
}

public class RefreshEndpoint : IEndpoint {
    private readonly IMediator _mediator;

    public RefreshEndpoint(IMediator mediator) {
        _mediator = Guard.Against.Null(mediator);
    }

    /// <inheritdoc />
    public IEndpointDescriptor Describe() {
        var builder = EndpointDescriptorBuilder.Create<RefreshEndpoint>();

        builder
            .Post("refresh", nameof(ExecuteAsync))
            .AllowAnonymous()
            .WithDescription("Refresh the user token with a new one, if the refresh token still valid.")
            .WithDisplayName("Refresh")
            .WithGroupName("auth")
            .Produces<RefreshOutput>()
            .ProducesProblem(statusCode: StatusCodes.Status401Unauthorized)
            .ProducesProblem(statusCode: StatusCodes.Status500InternalServerError)
            .ProducesValidationProblem();

        return builder.Build();
    }

    public async Task<IResult> ExecuteAsync([FromBody] RefreshInput input, CancellationToken cancellationToken) {
        Guard.Against.Null(input);

        var validateResponse = await ValidateRefreshTokenAsync(input.Token, cancellationToken);
        if (!validateResponse.Succeeded) {
            return validateResponse.Status switch {
                UserRefreshTokenStatus.Revoked or
                UserRefreshTokenStatus.Expired or
                UserRefreshTokenStatus.Inactive => Forbid(),
                _ => BadRequest(validateResponse.Error)
            };
        }

        var accessTokenResponse = await CreateAccessTokenAsync(validateResponse.UserID, cancellationToken);
        if (!accessTokenResponse.Succeeded) {
            return Forbid();
        }

        var refreshTokenResponse = await CreateRefreshTokenAsync(validateResponse.UserID, cancellationToken);

        return Ok(new RefreshOutput {
            AccessToken = accessTokenResponse.Token!,
            RefreshToken = refreshTokenResponse.Token
        });
    }

    private async Task<ValidateRefreshTokenResponse> ValidateRefreshTokenAsync(string token, CancellationToken cancellationToken) {
        var request = new ValidateRefreshTokenRequest {
            Token = token
        };
        return await _mediator.ExecuteAsync(request, cancellationToken)
                              .ConfigureAwait(continueOnCapturedContext: false);
    }

    private async Task<CreateAccessTokenResponse> CreateAccessTokenAsync(Guid userID, CancellationToken cancellationToken) {
        var request = new CreateAccessTokenRequest {
            UserID = userID
        };

        return await _mediator.ExecuteAsync(request, cancellationToken)
                              .ConfigureAwait(continueOnCapturedContext: false);
    }

    private async Task<CreateRefreshTokenResponse> CreateRefreshTokenAsync(Guid userID, CancellationToken cancellationToken) {
        var request = new CreateRefreshTokenRequest {
            UserID = userID
        };

        return await _mediator.ExecuteAsync(request, cancellationToken)
                              .ConfigureAwait(continueOnCapturedContext: false);
    }
}

using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Nameless.Mediator.Requests;
using Nameless.Web.Identity.Entities;
using Nameless.Web.Identity.Infrastructure;
using Nameless.Web.Identity.Internals;
using Nameless.Web.Identity.Jwt;
using Nameless.Web.IdentityModel.Jwt;

namespace Nameless.Web.Identity.UseCases.SecurityTokens.AccessToken;

public class CreateAccessTokenRequestHandler : IRequestHandler<CreateAccessTokenRequest, CreateAccessTokenResponse> {
    private readonly SignInManager<User> _signInManager;
    private readonly TimeProvider _timeProvider;
    private readonly IOptions<JsonWebTokenOptions> _options;
    private readonly ILogger<CreateAccessTokenRequestHandler> _logger;

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="CreateAccessTokenRequestHandler"/> class.
    /// </summary>
    /// <param name="signInManager">
    ///     The identity sign-in manager.
    /// </param>
    /// <param name="timeProvider">
    ///     The time provider.
    /// </param>
    /// <param name="options">
    ///     The options.
    /// </param>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    public CreateAccessTokenRequestHandler(
        SignInManager<User> signInManager,
        TimeProvider timeProvider,
        IOptions<JsonWebTokenOptions> options,
        ILogger<CreateAccessTokenRequestHandler> logger) {
        _signInManager = Guard.Against.Null(signInManager);
        _timeProvider = Guard.Against.Null(timeProvider);
        _options = Guard.Against.Null(options);
        _logger = Guard.Against.Null(logger);
    }

    /// <inheritdoc />
    public async Task<CreateAccessTokenResponse> HandleAsync(CreateAccessTokenRequest request, CancellationToken cancellationToken) {
        Guard.Against.Null(request);

        try {
            var user = await _signInManager.UserManager
                                           .FindByIdAsync(request.UserID.ToString())
                                           .ConfigureAwait(continueOnCapturedContext: false)
                       ?? throw new UserNotFoundException();

            var principal = await _signInManager.CreateUserPrincipalAsync(user)
                                                .ConfigureAwait(continueOnCapturedContext: false);
            var descriptor = CreateSecurityTokenDescriptor(principal.Claims);
            var tokenHandler = new JsonWebTokenHandler {
                MapInboundClaims = false
            };

            // Force JsonWebTokenHandler to use the default claim name
            // https://stackoverflow.com/questions/57998262/why-is-claimtypes-nameidentifier-not-mapping-to-sub
            tokenHandler.InboundClaimTypeMap.Clear();

            var token = tokenHandler.CreateToken(descriptor);

            return new CreateAccessTokenResponse { Token = token };
        }
        catch (MissingSecretConfigurationException) { _logger.MissingSecretConfiguration(); throw; }
        catch (MissingClaimSubException) { _logger.MissingClaimSub(); throw; }
        catch (Exception ex) { _logger.CreateJsonWebTokenFailure(ex); throw; }
    }

    private SecurityTokenDescriptor CreateSecurityTokenDescriptor(IEnumerable<Claim> claims) {
        var options = _options.Value;

        if (string.IsNullOrWhiteSpace(options.Secret)) {
            throw new MissingSecretConfigurationException();
        }

        var innerClaims = claims.ToArray();
        if (innerClaims.All(claim => claim.Type != MS_JwtRegisteredClaimNames.Sub)) {
            throw new MissingClaimSubException();
        }

        var result = new SecurityTokenDescriptor {
            Claims = new Dictionary<string, object>(),
            SigningCredentials = new SigningCredentials(
                key: new SymmetricSecurityKey(options.Secret.GetBytes()),
                algorithm: SecurityAlgorithms.HmacSha256
            ),
        };

        if (!string.IsNullOrEmpty(options.Issuer)) {
            result.Claims[MS_JwtRegisteredClaimNames.Iss] = options.Issuer;
        }

        if (options.Audiences.Length > 0) {
            foreach (var audience in options.Audiences) {
                result.Audiences.Add(audience);
            }
        }

        InsertClaimCollection(result, options.AdditionalClaims);
        InsertClaimCollection(result, innerClaims);

        var now = _timeProvider.GetUtcNow();
        result.Claims[MS_JwtRegisteredClaimNames.Iat] = $"{now.ToUnixTimeSeconds()}";

        var expires = now.Add(options.TokenExpiresIn);
        result.Claims[MS_JwtRegisteredClaimNames.Exp] = $"{expires.ToUnixTimeSeconds()}";

        result.Claims[MS_JwtRegisteredClaimNames.Jti] = $"{Guid.NewGuid():N}";

        return result;
    }

    private static void InsertClaimCollection(SecurityTokenDescriptor result, IEnumerable<Claim> claims) {
        foreach (var claimGroup in claims.GroupBy(claim => claim.Type)) {
            var value = claimGroup.Count() switch {
                < 1 => null,
                > 1 => JsonSerializer.Serialize(claimGroup.Select(claim => claim.Value)),
                _ => claimGroup.First().Value
            };

            if (value is null) {
                continue;
            }

            result.Claims[claimGroup.Key] = value;
        }
    }
}
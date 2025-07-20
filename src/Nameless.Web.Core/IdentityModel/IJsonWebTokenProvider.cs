using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Nameless.Web.IdentityModel;

/// <summary>
///     Represents a JSON Web Token response.
/// </summary>
public record JsonWebTokenResponse {
    /// <summary>
    ///     Gets or init the token.
    /// </summary>
    public string? Token { get; init; }

    /// <summary>
    ///     Gets or init the error message.
    /// </summary>
    public string? Error { get; init; }

    /// <summary>
    ///     Whether it was able to generate the token.
    /// </summary>
    [MemberNotNullWhen(returnValue: true, nameof(Token))]
    [MemberNotNullWhen(returnValue: false, nameof(Error))]
    public bool Succeeded => string.IsNullOrWhiteSpace(Error);
}

/// <summary>
///     Represents a request for a JSON Web Token.
/// </summary>
public record JsonWebTokenRequest {
    /// <summary>
    ///     Gets or sets the claims for the JSON Web Token.
    /// </summary>
    public IEnumerable<Claim> Claims { get; set; } = [];
}

public interface IJsonWebTokenProvider {
    /// <summary>
    ///     Generates a JSON Web Token.
    /// </summary>
    /// <param name="request">
    ///     A request object containing all necessary information to generate
    ///     the JSON Web Token.
    /// </param>
    /// <returns>
    ///     A <see cref="JsonWebTokenResponse"/> containing the generated
    ///     access token.
    /// </returns>
    JsonWebTokenResponse Create(JsonWebTokenRequest request);
}
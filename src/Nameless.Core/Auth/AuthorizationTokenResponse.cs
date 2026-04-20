using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Auth;

/// <summary>
///     Represents an authorization token response.
/// </summary>
/// <typeparam name="TToken">
///     Type of the token.
/// </typeparam>
public sealed class AuthorizationTokenResponse<TToken> : Result<TToken>
    where TToken : notnull {
    private AuthorizationTokenResponse(TToken? value, Error[] errors)
        : base(value, errors) {
    }

    /// <summary>
    ///     Converts a <typeparamref name="TToken"/> into a
    ///     <see cref="AuthorizationTokenResponse{TToken}"/> instance.
    /// </summary>
    /// <param name="value">
    ///     The token.
    /// </param>
    public static implicit operator AuthorizationTokenResponse<TToken>(TToken value) {
        return new AuthorizationTokenResponse<TToken>(value, errors: []);
    }

    /// <summary>
    ///     Converts an <see cref="Error"/> into a
    ///     <see cref="AuthorizationTokenResponse{TToken}"/> instance.
    /// </summary>
    /// <param name="error">
    ///     The error.
    /// </param>
    public static implicit operator AuthorizationTokenResponse<TToken>(Error error) {
        return new AuthorizationTokenResponse<TToken>(value: default, errors: [error]);
    }
}
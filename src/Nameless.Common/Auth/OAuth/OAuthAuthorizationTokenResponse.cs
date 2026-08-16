using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Auth.OAuth;

/// <summary>
///     Represents an OAuth authorization token response.
/// </summary>
public sealed class OAuthAuthorizationTokenResponse : Result<OAuthAuthorizationToken> {
    private OAuthAuthorizationTokenResponse(OAuthAuthorizationToken? value, Error[] errors)
        : base(value, errors) { }

    /// <summary>
    ///     Converts a <see cref="OAuthAuthorizationToken"/> into a
    ///     <see cref="OAuthAuthorizationTokenResponse"/> instance.
    /// </summary>
    /// <param name="value">
    ///     The token.
    /// </param>
    public static implicit operator OAuthAuthorizationTokenResponse(OAuthAuthorizationToken value) {
        return new OAuthAuthorizationTokenResponse(value, errors: []);
    }

    /// <summary>
    ///     Converts an <see cref="Error"/> into a
    ///     <see cref="OAuthAuthorizationTokenResponse"/> instance.
    /// </summary>
    /// <param name="error">
    ///     The error.
    /// </param>
    public static implicit operator OAuthAuthorizationTokenResponse(Error error) {
        return new OAuthAuthorizationTokenResponse(value: null, errors: [error]);
    }

    /// <summary>
    ///     Converts an <see cref="Error"/> array into a
    ///     <see cref="OAuthAuthorizationTokenResponse"/> instance.
    /// </summary>
    /// <param name="errors">
    ///     The error array.
    /// </param>
    public static implicit operator OAuthAuthorizationTokenResponse(Error[] errors) {
        return new OAuthAuthorizationTokenResponse(value: null, errors);
    }
}
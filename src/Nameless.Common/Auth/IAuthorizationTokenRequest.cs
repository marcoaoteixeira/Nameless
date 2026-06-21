namespace Nameless.Auth;

/// <summary>
///     Represents an authorization token request.
/// </summary>
public interface IAuthorizationTokenRequest<TResponse> {
    /// <summary>
    ///     Gets the scheme it should use.
    /// </summary>
    string? Scheme { get; init; }
}
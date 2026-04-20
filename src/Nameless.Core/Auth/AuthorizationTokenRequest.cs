namespace Nameless.Auth;

/// <summary>
///     Represents an authorization token request.
/// </summary>
public record AuthorizationTokenRequest {
    /// <summary>
    ///     Gets the scheme it should use.
    /// </summary>
    public string? Scheme { get; init; }
}
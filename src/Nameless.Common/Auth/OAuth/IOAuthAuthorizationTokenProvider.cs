namespace Nameless.Auth.OAuth;

/// <summary>
///     OAuth authorization token provider.
/// </summary>
public interface IOAuthAuthorizationTokenProvider : IAuthorizationTokenProvider<OAuthAuthorizationTokenRequest, OAuthAuthorizationTokenResponse>;
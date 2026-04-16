namespace Nameless.Auth;

public interface IAuthorizationTokenProvider<in TRequest, TToken>
    where TRequest : AuthorizationTokenRequest
    where TToken : notnull {
    Task<AuthorizationTokenResponse<TToken>> GetTokenAsync(TRequest request, CancellationToken cancellationToken);
}
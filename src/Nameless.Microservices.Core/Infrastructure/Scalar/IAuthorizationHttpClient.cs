namespace Nameless.Microservices.Infrastructure.Scalar;

public interface IAuthorizationHttpClient {
    Task<TToken> GetTokenAsync<TToken>(string authorizationScheme, CancellationToken cancellationToken);
}
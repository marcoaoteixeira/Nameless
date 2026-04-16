using Microsoft.Extensions.Logging;

namespace Nameless.Auth;

internal static class LoggerExtensions {
    extension<TRequest, TToken>(ILogger<AuthorizationTokenProvider<TRequest, TToken>> self)
        where TRequest : AuthorizationTokenRequest
        where TToken : notnull {
        internal void Failure(Exception exception) {
            Log.Failure(
                self,
                "AUTHORIZATION_TOKEN_PROVIDER",
                $"{self.GetType().GetPrettyName()}.{nameof(AuthorizationTokenProvider<,>.GetTokenAsync)}",
                exception
            );
        }
    }
}
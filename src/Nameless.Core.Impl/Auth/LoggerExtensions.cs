using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Nameless.Auth;

[ExcludeFromCodeCoverage(Justification = CodeCoverage.Justifications.InternalCode)]
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
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Nameless.Diagnostics.CodeAnalysis;

namespace Nameless.Auth.OAuth;

[ExcludeFromCodeCoverage(Justification = CodeCoverage.Justifications.InternalCode)]
internal static class LoggerExtensions {
    private const string TAG = "AUTHORIZATION_TOKEN_PROVIDER";

    extension(ILogger<OAuthAuthorizationTokenProvider> self) {
        internal void Failure(Exception exception) {
            Log.Failure(
                logger: self,
                tag: TAG,
                actionName: nameof(OAuthAuthorizationTokenProvider.GetTokenAsync),
                exception: exception
            );
        }

        internal void GetTokenAsyncFailure(Exception exception) {
            Log.Failure(
                logger: self,
                tag: TAG,
                actionName: nameof(OAuthAuthorizationTokenProvider.GetTokenAsync),
                exception: exception
            );
        }

        internal void DeserializeTokenAsyncWarning(string reason) {
            Log.Warning(
                logger: self,
                tag: TAG,
                actionName: "DeserializeTokenAsync",
                reason
            );
        }

        internal void DeserializeTokenAsyncFailure(Exception exception) {
            Log.Failure(
                logger: self,
                tag: TAG,
                actionName: "DeserializeTokenAsync",
                exception
            );
        }
    }
}
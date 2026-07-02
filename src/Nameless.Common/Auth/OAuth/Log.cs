using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Nameless.Diagnostics.CodeAnalysis;

namespace Nameless.Auth.OAuth;

[ExcludeFromCodeCoverage(Justification = CodeCoverage.Justifications.InternalCode)]
internal static partial class Log {
    private const string TAG = "OAUTH_AUTHORIZATION_TOKEN_PROVIDER";

    [LoggerMessage(LogLevel.Error, message: "[{Tag}] An error has occurred while retrieve the authorization token.")]
    internal static partial void GetTokenAsyncFailure(ILogger<OAuthAuthorizationTokenProvider> logger, Exception exception, string tag = TAG);

    [LoggerMessage(LogLevel.Warning, message: "[{Tag}] A problem has occurred while deserializing token that might need attention. Reason: {Reason}")]
    internal static partial void DeserializeTokenAsyncWarning(ILogger<OAuthAuthorizationTokenProvider> logger, string reason, string tag = TAG);

    [LoggerMessage(LogLevel.Error, message: "[{Tag}] An error has occurred while deserializing the authorization token.")]
    internal static partial void DeserializeTokenAsyncFailure(ILogger<OAuthAuthorizationTokenProvider> logger, Exception exception, string tag = TAG);
}
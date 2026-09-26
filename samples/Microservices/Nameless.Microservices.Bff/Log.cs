using Scalar.AspNetCore;

namespace Nameless.Microservices.Bff;

// Why? https://learn.microsoft.com/en-us/dotnet/core/extensions/logging/high-performance-logging
internal static partial class Log
{
    #region Common

    [LoggerMessage(level: LogLevel.Error, message: "[{Tag}] An error has occurred while executing action '{ActionName}'.")]
    internal static partial void Failure(ILogger logger, string tag, string actionName, Exception exception);

    #endregion

    #region Scalar Authentication

    [LoggerMessage(level: LogLevel.Information, message: "[SCALAR] Scalar HTTP Security Scheme failed to retrieve the access token. Reason: {Reason}")]
    internal static partial void ScalarUnableRetrieveAccessToken(ILogger<ScalarSecurityScheme> logger, string reason);

    #endregion
}

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Nameless.Diagnostics.CodeAnalysis;

namespace Nameless.GitHub;

[ExcludeFromCodeCoverage(Justification = CodeCoverage.Justifications.AutoGen)]
internal static partial class Log {
    private const string TAG = "GITHUB_CLIENT";

    [LoggerMessage(LogLevel.Error, message: "[{Tag}] An error has occurred while retrieving information about the latest release. Status code: {StatusCode}")]
    internal static partial void GetLastestReleaseAsyncFailure(ILogger<GitHubHttpClient> logger, int statusCode, Exception exception, string tag = TAG);

    [LoggerMessage(LogLevel.Error, message: "[{Tag}] An error has occurred while retrieving information about release assets. Status code: {StatusCode}")]
    internal static partial void GetReleaseAssetsAsyncFailure(ILogger<GitHubHttpClient> logger, int statusCode, Exception exception, string tag = TAG);

    [LoggerMessage(LogLevel.Warning, message: "[{Tag}] Unable to deserialize object '{ObjectName}' from response content. Status code: {StatusCode}")]
    internal static partial void ResponseDeserializationWarning(ILogger<GitHubHttpClient> logger, string objectName, int statusCode, string tag = TAG);
}
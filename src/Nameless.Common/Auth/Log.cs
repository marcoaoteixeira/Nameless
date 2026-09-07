using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Nameless.Diagnostics.CodeAnalysis;

namespace Nameless.Auth;

/// <summary>
///     Why? See: https://learn.microsoft.com/en-us/dotnet/core/extensions/logging/high-performance-logging
/// </summary>
[ExcludeFromCodeCoverage(Justification = CodeCoverage.Justifications.AutoGenCode)]
internal static partial class Log {
    internal const string DeserializationFailureMsg = "[{Tag}] Unable to deserialize JSON response content to '{Type}'.";

    [LoggerMessage(level: LogLevel.Error, message: DeserializationFailureMsg)]
    internal static partial void DeserializationFailure(ILogger logger, string type, string? tag = null);
}

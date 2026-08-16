using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Nameless.Diagnostics.CodeAnalysis;
using Nameless.ObjectModel;

namespace Nameless.Mediator.Pipelines;

[ExcludeFromCodeCoverage(Justification = CodeCoverage.Justifications.AutoGen)]
internal static partial class Log {
    private const string TAG = "MEDIATOR";

    [LoggerMessage(LogLevel.Debug, message: "[{Tag}] Request Handler <{RequestType},{ResponseType}> execution took {Duration}ms")]
    internal static partial void Complete(ILogger logger, string requestType, string responseType, long duration, string tag = TAG);

    [LoggerMessage(LogLevel.Debug, message: "[{Tag}] Validation pipeline failed with errors: {@Errors}")]
    internal static partial void ValidationFailure(ILogger logger, Error[] errors, string tag = TAG);
}
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Nameless.Diagnostics.CodeAnalysis;
using Nameless.ObjectModel;

namespace Nameless.Mediator.Pipelines;

[ExcludeFromCodeCoverage(Justification = CodeCoverage.Justifications.AutoGenCode)]
internal static partial class Log {

    [LoggerMessage(LogLevel.Debug, message: "[{Tag}] Validation pipeline failed with errors: {@Errors}")]
    internal static partial void ValidationFailure(ILogger logger, Error[] errors, string? tag = null);
}
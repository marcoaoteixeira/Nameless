using System.Data;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Nameless.Diagnostics.CodeAnalysis;

namespace Nameless.Data;

[ExcludeFromCodeCoverage(Justification = CodeCoverage.Justifications.AutoGenCode)]
internal static partial class Log {
    private const string TAG = "DATABASE";

    private const string DB_COMMAND_DEBUG_MSG = """
                                                [{Tag}] Executing:
                                                    Command: '{CommandText}'
                                                    Parameters: {@Parameters}
                                                """;
    
    [LoggerMessage(LogLevel.Debug, message: DB_COMMAND_DEBUG_MSG)]
    internal static partial void OutputDbCommandForDebug(ILogger<Database> logger, string commandText, IDataParameterCollection parameters, string tag = TAG);
}
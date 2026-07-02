using Microsoft.Extensions.Logging;

namespace Nameless.ComponentModel;

internal static partial class Log {
    private const string TAG = "ASSEMBLY_TYPE_CONVERTER";

    [LoggerMessage(LogLevel.Error, message: "[{Tag}] An error has occurred while loading assembly '{Assembly}'.")]
    internal static partial void ConvertToFailure(ILogger<AssemblyTypeConverter> logger, string assembly, Exception exception, string tag = TAG);
}
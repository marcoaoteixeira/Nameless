using Microsoft.Extensions.Logging;
using Nameless.Testing.Tools.Mockers.Logging;

namespace Nameless;

internal static partial class Log {
    [LoggerMessage(level: LogLevel.Debug, message: "Log Debug: {State}")]
    internal static partial void LogWithDebug(ILogger logger, string state);

    [LoggerMessage(level: LogLevel.Error, message: "Log Error: {State}")]
    internal static partial void LogWithError(ILogger logger, string state);
}

public class MySimpleService(ILogger logger) {
    public void DoSomething() {
        Log.LogWithDebug(logger, "Doing something...");
    }

    public void OhNoError() {
        Log.LogWithError(logger, "X_X dead...");
    }

    public void PreviousLog() {
        logger.LogWarning("Previous log");
    }
}

public class MySimpleServiceTests {
    [Fact]
    public void Check_Logger_Verification_Debug() {
        var loggerMocker = new LoggerMocker<MySimpleService>()
            .WithLogDebug();

        new MySimpleService(loggerMocker.Build()).DoSomething();

        loggerMocker.VerifyDebug(message => message.Contains("Debug"));
    }

    [Fact]
    public void Check_Logger_Verification_Error() {
        var loggerMocker = new LoggerMocker<MySimpleService>()
            .WithLogError();

        new MySimpleService(loggerMocker.Build()).OhNoError();

        loggerMocker.VerifyError(message => message.Contains("dead"));
    }

    [Fact]
    public void Check_Logger_Verification_Warning() {
        var loggerMocker = new LoggerMocker<MySimpleService>()
            .WithLogWarning();

        new MySimpleService(loggerMocker.Build()).PreviousLog();

        loggerMocker.VerifyWarning(message => message.Contains("Previous"));
    }
}
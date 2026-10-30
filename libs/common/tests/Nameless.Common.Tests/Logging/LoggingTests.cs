using Nameless.Testing.Tools.Logging;

namespace Nameless.Logging;

[UnitTest]
public class StopwatchLoggerFormattingTests {
    public sealed class Sample;

    [Fact]
    public void StartStopwatchLogger_WithProxyLogger_UsesUnknownClassName() {
        // arrange
        var logger = new InMemoryLogger<Sample>();

        // act
        using (logger.StartStopwatchLogger(caller: "Action")) { }

        // assert
        logger.VerifyDebug(message => message.Contains("Unknown", StringComparison.Ordinal) && message.Contains("Action", StringComparison.Ordinal), times: 2);
    }

    [Fact]
    public void StartStopwatchLogger_MessageFormat_PutsDurationBeforeMessage() {
        // arrange
        var logger = new InMemoryLogger<Sample>();

        // act
        using (var stopwatch = logger.StartStopwatchLogger(caller: "Action")) {
            stopwatch.Write("custom-message");
        }

        // assert
        logger.VerifyDebug(message => message.EndsWith("custom-message", StringComparison.Ordinal));
    }

    [Fact]
    public void StartStopwatchLogger_WithDisabledLevel_DoesNotLog() {
        // arrange
        var logger = new InMemoryLogger<Sample>();

        // act
        var exception = Record.Exception(() => { using var _ = logger.StartStopwatchLogger(); });

        // assert
        Assert.Null(exception);
    }

    [Fact]
    public void Write_AfterDispose_IsIgnored() {
        // arrange
        var logger = new InMemoryLogger<Sample>();
        var stopwatch = logger.StartStopwatchLogger();
        stopwatch.Dispose();

        // act
        var exception = Record.Exception(() => {
            stopwatch.Write("ignored");
            stopwatch.Dispose();
        });

        // assert
        Assert.Null(exception);
    }

    [Fact]
    public void StartStopwatchLogger_WithNonGenericLogger_UsesUnknownOrCategoryName() {
        // arrange
        var logger = Microsoft.Extensions.Logging.Abstractions.NullLogger.Instance;

        // act
        var exception = Record.Exception(() => { using var _ = logger.StartStopwatchLogger(); });

        // assert
        Assert.Null(exception);
    }
}
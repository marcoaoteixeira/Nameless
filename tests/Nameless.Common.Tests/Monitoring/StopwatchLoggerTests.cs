using System.Reflection;
using Microsoft.Extensions.Logging;
using Nameless.Logging;
using Nameless.Testing.Tools.Mockers.Logging;

namespace Nameless.Monitoring;

[UnitTest]
public class StopwatchLoggerTests {
    [Fact]
    public void WhenCreateStopwatchCounter_ThenLogEvents() {
        // arrange
        var loggerMocker = new LoggerMocker<StopwatchLoggerTests>().WithAnyLogLevel();
        var logger = loggerMocker.Build();
        var sut = Activator.CreateInstance(
            type: typeof(StopwatchLogger),
            bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            args: [
                logger,
                nameof(StopwatchLoggerTests),
                nameof(WhenCreateStopwatchCounter_ThenLogEvents),
                LogLevel.Debug
            ],
            culture: null
        ) as StopwatchLogger;

        if (sut is null) { Assert.Fail("Sut is null"); }

        // act
        sut.Write("This is a test");
        sut.Dispose();
        
        // assert
        loggerMocker.VerifyDebug(message => message.Contains("starting"));
        loggerMocker.VerifyDebug(message => message.Contains("This is a test"));
        loggerMocker.VerifyDebug(message => message.Contains("finished"));
    }
}

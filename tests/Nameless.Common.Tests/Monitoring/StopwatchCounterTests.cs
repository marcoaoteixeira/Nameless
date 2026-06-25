using System.Reflection;
using Microsoft.Extensions.Logging;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Mockers.Logging;

namespace Nameless.Monitoring;

[UnitTest]
public class StopwatchCounterTests {
    [Fact]
    public void WhenCreateStopwatchCounter_ThenLogEvents() {
        // arrange
        var loggerMocker = new LoggerMocker<StopwatchCounterTests>().WithAnyLogLevel();
        var logger = loggerMocker.Build();
        var sut = Activator.CreateInstance(
            type: typeof(StopwatchCounter),
            bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            args: [
                logger,
                nameof(StopwatchCounterTests),
                nameof(WhenCreateStopwatchCounter_ThenLogEvents),
                LogLevel.Debug
            ],
            culture: null
        ) as StopwatchCounter;

        if (sut is null) { Assert.Fail("Sut is null"); }

        // act
        sut.Echo("This is a test");
        sut.Dispose();
        
        // assert
        loggerMocker.VerifyDebug(message => message.Contains("starting"));
        loggerMocker.VerifyDebug(message => message.Contains("This is a test"));
        loggerMocker.VerifyDebug(message => message.Contains("finished"));
    }
}

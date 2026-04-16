#pragma warning disable CA1873

using Microsoft.Extensions.Logging;
using Moq;

namespace Nameless.Testing.Tools.Mockers.Logging;

public class LoggerMocker<T> : Mocker<ILogger<T>> {
    public LoggerMocker<T> WithAnyLogLevel() {
        MockInstance.Setup(mock => mock.IsEnabled(It.IsAny<LogLevel>()))
                    .Returns(value: true);

        return this;
    }

    public LoggerMocker<T> WithIsEnabled(params LogLevel[] levels) {
        foreach (var level in levels) {
            MockInstance.Setup(mock => mock.IsEnabled(level))
                        .Returns(value: true);
        }

        return this;
    }

    public LoggerMocker<T> WithLog(LogLevel level = LogLevel.Information, Action<string>? callback = null) {
        var flow = MockInstance.Setup(mock => mock.Log(level,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception?>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>())
        );

        if (callback is not null) {
            flow.Callback(new InvocationAction(invocation => {
                var state = invocation.Arguments[index: 2];
                var formatter = invocation.Arguments[index: 4];
                var invoker = formatter.GetType().GetMethod(name: "Invoke");
                var message = (string)invoker!.Invoke(formatter, [state, null])!;

                callback(message);
            }));
        }

        return this;
    }

    public Mocker<ILogger<T>> VerifyDebug(Func<string, bool>? assertMessage = null, int times = 1, bool exactly = true) {
        return VerifyCallFor(assertMessage, LogLevel.Debug, times, exactly);
    }

    public Mocker<ILogger<T>> VerifyError(Func<string, bool>? assertMessage = null, int times = 1, bool exactly = true) {
        return VerifyCallFor(assertMessage, LogLevel.Error, times, exactly);
    }

    public Mocker<ILogger<T>> VerifyInformation(Func<string, bool>? assertMessage = null, int times = 1, bool exactly = true) {
        return VerifyCallFor(assertMessage, LogLevel.Information, times, exactly);
    }

    public Mocker<ILogger<T>> VerifyWarning(Func<string, bool>? assertMessage = null, int times = 1, bool exactly = true) {
        return VerifyCallFor(assertMessage, LogLevel.Warning, times, exactly);
    }

    private Mocker<ILogger<T>> VerifyCallFor(Func<string, bool>? assertMessage, LogLevel level, int times, bool exactly) {
        Func<object, Type, bool> state = (value, type)
            => (assertMessage ?? (_ => true)).Invoke(value.ToString() ?? string.Empty) &&
               type.Name.Contains(value: "LogValues", StringComparison.OrdinalIgnoreCase);

        MockInstance.Verify(mock => mock.Log(
            It.Is<LogLevel>(currentLogLevel => currentLogLevel == level),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((value, type) => state(value, type)),
            It.IsAny<Exception?>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), exactly ? Times.Exactly(times) : Times.AtLeast(times));

        return this;
    }
}
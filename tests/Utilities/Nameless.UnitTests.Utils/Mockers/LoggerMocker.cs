using Microsoft.Extensions.Logging;
using Moq;

namespace Nameless.Mockers;

public class LoggerMocker<T> : MockerBase<ILogger<T>> {
    public LoggerMocker<T> WithLogLevel(LogLevel logLevel, bool isEnabled = true) {
        InnerMock.Setup(mock => mock.IsEnabled(logLevel))
                 .Returns(isEnabled);

        return this;
    }

    public MockerBase<ILogger<T>> VerifyDebugCall(Func<string, bool> assertMessage = null, Times times = default)
        => VerifyCallFor(assertMessage, LogLevel.Debug, times);

    public MockerBase<ILogger<T>> VerifyErrorCall(Func<string, bool> assertMessage = null, Times times = default)
        => VerifyCallFor(assertMessage, LogLevel.Error, times);

    public MockerBase<ILogger<T>> VerifyInformationCall(Func<string, bool> assertMessage = null, Times times = default)
        => VerifyCallFor(assertMessage, LogLevel.Information, times);

    public MockerBase<ILogger<T>> VerifyWarningCall(Func<string, bool> assertMessage = null, Times times = default)
        => VerifyCallFor(assertMessage, LogLevel.Warning, times);

    public MockerBase<ILogger<T>> VerifyCallFor(Func<string, bool> assertMessage, LogLevel level = LogLevel.Debug, Times times = default) {
        Func<object, Type, bool> state = (value, type)
            => (assertMessage ?? (_ => true)).Invoke(value.ToString() ?? string.Empty) &&
               type.Name.Contains("LogValues", StringComparison.OrdinalIgnoreCase);

        InnerMock.Verify(mock => mock.Log(It.Is<LogLevel>(currentLogLevel => currentLogLevel == level),
                                          It.Is<EventId>(eventId => eventId.Id == 0),
                                          It.Is<It.IsAnyType>((value, type) => state(value, type)),
                                          It.IsAny<Exception>(),
                                          It.IsAny<Func<It.IsAnyType, Exception, string>>()), times);

        return this;
    }
}
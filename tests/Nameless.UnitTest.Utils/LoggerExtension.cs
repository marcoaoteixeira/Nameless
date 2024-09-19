using Microsoft.Extensions.Logging;
using Moq;

namespace Nameless;

public static class LoggerExtension {
    private static readonly Func<string, bool> EmptyAssertMessage = _ => true;

    public static Mock<ILogger<T>> VerifyDebugCall<T>(this Mock<ILogger<T>> self, Func<string, bool> assertMessage = null, Times times = default)
        => VerifyCallFor(self, assertMessage ?? EmptyAssertMessage, LogLevel.Debug, times);

    public static Mock<ILogger<T>> VerifyErrorCall<T>(this Mock<ILogger<T>> self, Func<string, bool> assertMessage = null, Times times = default)
        => VerifyCallFor(self, assertMessage ?? EmptyAssertMessage, LogLevel.Error, times);

    public static Mock<ILogger<T>> VerifyInformationCall<T>(this Mock<ILogger<T>> self, Func<string, bool> assertMessage = null, Times times = default)
        => VerifyCallFor(self, assertMessage ?? EmptyAssertMessage, LogLevel.Information, times);

    public static Mock<ILogger<T>> VerifyWarningCall<T>(this Mock<ILogger<T>> self, Func<string, bool> assertMessage = null, Times times = default)
        => VerifyCallFor(self, assertMessage ?? EmptyAssertMessage, LogLevel.Warning, times);

    public static Mock<ILogger<T>> VerifyCallFor<T>(this Mock<ILogger<T>> self, Func<string, bool> assertMessage, LogLevel level = LogLevel.Debug, Times times = default) {
        Func<object, Type, bool> state = (value, type)
            => (assertMessage ?? EmptyAssertMessage)(value.ToString() ?? string.Empty) && type.Name.Contains("LogValues", StringComparison.OrdinalIgnoreCase);

        self.Verify(mock => mock.Log(It.Is<LogLevel>(currentLogLevel => currentLogLevel == level),
                                     It.Is<EventId>(eventId => eventId.Id == 0),
                                     It.Is<It.IsAnyType>((value, type) => state(value, type)),
                                     It.IsAny<Exception>(),
                                     It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                    times);

        return self;
    }
}
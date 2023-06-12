namespace Nameless.Logging.log4net {

    public readonly record struct LogMessage(LogLevel LogLevel, string Message, Exception? Exception = default, object[]? Args = default);
}

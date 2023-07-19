namespace Nameless.Logging.log4net {
    public readonly record struct LogMessage(Level Level, string Message, Exception? Exception = default, object[]? Args = default);
}

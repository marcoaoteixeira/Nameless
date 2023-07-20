using L4N_ILogger = log4net.Core.ILogger;
using L4N_Level = log4net.Core.Level;
using L4N_LoggingEvent = log4net.Core.LoggingEvent;

namespace Nameless.Logging.log4net {
    public sealed class LoggerEventFactory : ILoggerEventFactory {
        #region ILoggerEventFactory Members

        public L4N_LoggingEvent? CreateLoggingEvent(in LogMessage message, L4N_ILogger logger, Log4netOptions options) {
            Prevent.Against.Null(message, nameof(message));
            Prevent.Against.Null(logger, nameof(logger));

            var level = LevelTranslator.Translate(message.Level, options.OverrideCriticalLevel);
            if (level == L4N_Level.Off) {
                return null;
            }

            var outputMessage = message.Args.IsNullOrEmpty()
                ? message.Message
                : string.Format(message.Message, message.Args ?? Array.Empty<object>());

            if (string.IsNullOrWhiteSpace(outputMessage) && message.Exception == null) {
                return null;
            }

            var result = new L4N_LoggingEvent(
                callerStackBoundaryDeclaringType: typeof(LoggerEventFactory),
                repository: logger.Repository,
                loggerName: logger.Name,
                level: level,
                message: outputMessage,
                exception: message.Exception
            );

            return result;
        }

        #endregion
    }
}

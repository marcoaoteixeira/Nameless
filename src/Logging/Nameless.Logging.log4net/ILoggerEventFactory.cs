using L4N_ILogger = log4net.Core.ILogger;
using L4N_LoggingEvent = log4net.Core.LoggingEvent;

namespace Nameless.Logging.log4net {
    public interface ILoggerEventFactory {
        #region Methods

        L4N_LoggingEvent? CreateLoggingEvent(in LogMessage message, L4N_ILogger logger, Log4netOptions options);

        #endregion
    }
}

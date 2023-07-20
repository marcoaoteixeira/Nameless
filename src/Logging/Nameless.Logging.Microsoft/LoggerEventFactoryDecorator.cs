using System.Collections;
using System.Globalization;
using Nameless.Logging.log4net;

namespace Nameless.Logging.Microsoft {
    public sealed class LoggerEventFactoryDecorator : ILoggerEventFactory {
        #region Private Constants

        private const string DEFAULT_SCOPE_PROPERTY = "scope";

        #endregion

        #region Private Read-Only Fields

        private readonly ILoggerEventFactory _factory;
        private readonly IMSExternalScopeProvider _externalScopeProvider;

        #endregion

        #region Public Constructors

        public LoggerEventFactoryDecorator(ILoggerEventFactory factory, IMSExternalScopeProvider externalScopeProvider) {
            _factory = Prevent.Against.Null(factory, nameof(factory));
            _externalScopeProvider = Prevent.Against.Null(externalScopeProvider, nameof(externalScopeProvider));
        }

        #endregion

        #region Private Static Methods

        private static void Enrich(L4NLoggingEvent loggingEvent, IMSExternalScopeProvider externalScopeProvider) {
            static string? join(string? previous, string? actual) {
                return string.IsNullOrEmpty(previous)
                    ? actual
                    : string.Concat(previous, Constants.Separators.Space, actual);
            }

            externalScopeProvider.ForEachScope((scope, @event) => {
                // This function will add the scopes in the legacy way they were added before the IExternalScopeProvider was introduced,
                // to maintain backwards compatibility.
                // This pretty much means that we are emulating a LogicalThreadContextStack, which is a stack, that allows pushing
                // strings on to it, which will be concatenated with space as a separator.
                // See: https://github.com/apache/logging-log4net/blob/47aaf46d5f031ea29d781bac4617bd1bb9446215/src/log4net/Util/LogicalThreadContextStack.cs#L343

                // Because string implements IEnumerable we first need to check for string.
                if (scope is string) {
                    var previousValue = @event.Properties[DEFAULT_SCOPE_PROPERTY] as string;
                    @event.Properties[DEFAULT_SCOPE_PROPERTY] = join(previousValue, scope as string);
                    return;
                }

                if (scope is IEnumerable collection) {
                    foreach (var item in collection) {
                        switch (item) {
                            case KeyValuePair<string, string>: {
                                    var keyValuePair = (KeyValuePair<string, string>)item;
                                    var previousValue = @event.Properties[keyValuePair.Key] as string;
                                    @event.Properties[keyValuePair.Key] = join(previousValue, keyValuePair.Value);
                                    continue;
                                }

                            case KeyValuePair<string, object>: {
                                    var keyValuePair = (KeyValuePair<string, object>)item;
                                    var previousValue = @event.Properties[keyValuePair.Key] as string;

                                    // The current culture should not influence how integers/floats/... are displayed in logging,
                                    // so we are using Convert.ToString which will convert IConvertible and IFormattable with
                                    // the specified IFormatProvider.
                                    var additionalValue = Convert.ToString(keyValuePair.Value, CultureInfo.InvariantCulture);
                                    @event.Properties[keyValuePair.Key] = join(previousValue, additionalValue);
                                    continue;
                                }
                        }
                    }
                    return;
                }

                if (scope is not null) {
                    var previousValue = @event.Properties[DEFAULT_SCOPE_PROPERTY] as string;
                    var additionalValue = Convert.ToString(scope, CultureInfo.InvariantCulture);
                    @event.Properties[DEFAULT_SCOPE_PROPERTY] = join(previousValue, additionalValue);
                    return;
                }

            }, loggingEvent);
        }

        #endregion

        #region ILoggerEventFactory Members

        public L4NLoggingEvent? CreateLoggingEvent(in LogMessage message, IL4NLogger logger, Log4netOptions options) {
            var result = _factory.CreateLoggingEvent(message, logger, options);

            if (result == null) { return null; }

            Enrich(result, _externalScopeProvider);

            return result;
        }

        #endregion
    }
}

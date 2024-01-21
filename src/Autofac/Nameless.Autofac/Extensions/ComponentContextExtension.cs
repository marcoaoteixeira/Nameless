using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Nameless.Autofac {
    public static class ComponentContextExtension {
        #region Public Static Methods

        public static ILogger GetLogger<T>(this IComponentContext self) {
            var loggerFactory = self.ResolveOptional<ILoggerFactory>();

            return loggerFactory is not null
                ? loggerFactory.CreateLogger<T>()
                : NullLogger.Instance;
        }

        public static TOptions GetOptions<TOptions>(this IComponentContext self)
            where TOptions : class, new() {
            var configuration = self.ResolveOptional<IConfiguration>();
            var key = typeof(TOptions)
                .Name
                .RemoveTail(Root.Defaults.OptionsSettingsTails);

            TOptions? result = default;
            if (configuration is not null) {
                result = configuration.GetSection(key).Get<TOptions>();
            }
            return result ?? new();
        }

        #endregion
    }
}

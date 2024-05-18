using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Nameless {
    public static class ServiceProviderExtension {
        #region Public Static Methods

        public static ILogger<T> GetLogger<T>(this IServiceProvider self) {
            var loggerFactory = self.GetService<ILoggerFactory>();

            return loggerFactory is not null
                ? loggerFactory.CreateLogger<T>()
                : NullLogger<T>.Instance;
        }

        public static ILogger GetLogger(this IServiceProvider self, Type serviceType) {
            var loggerFactory = self.GetService<ILoggerFactory>();

            return loggerFactory is not null
                ? loggerFactory.CreateLogger(serviceType)
                : NullLogger.Instance;
        }

        public static TOptions GetPocoOptions<TOptions>(this IServiceProvider self)
            where TOptions : class, new() {
            // let's first check if we have it on our container
            var options = self.GetService<TOptions>();
            if (options is not null) {
                return options;
            }

            // ok, no good. let's try get from the configuration
            var configuration = self.GetService<IConfiguration>();
            var sectionName = typeof(TOptions)
                .Name
                .RemoveTail(Root.Defaults.OptionsSettingsTails);

            TOptions? result = default;
            if (configuration is not null) {
                result = configuration
                    .GetSection(sectionName)
                    .Get<TOptions>();
            }

            // returns from configuration or build.
            return result ?? new TOptions();
        }

        #endregion
    }
}

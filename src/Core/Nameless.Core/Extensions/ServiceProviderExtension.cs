using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Nameless {
    public static class ServiceProviderExtension {
        #region Public Static Methods

        public static bool TryGetService<TService>(this IServiceProvider self, [NotNullWhen(returnValue: true)] out TService? service) {
            service = self.GetService<TService>();

            return service is not null;
        }

        public static bool TryGetKeyedService<TService>(this IServiceProvider self, string key, [NotNullWhen(returnValue: true)] out TService? service) {
            service = self.GetKeyedService<TService>(key);

            return service is not null;
        }

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

        public static IOptions<TOptions> GetOptions<TOptions>(this IServiceProvider self, Func<TOptions>? factory = null)
            where TOptions : class {
            // let's first check if our provider can resolve this option
            var options = self.GetService<IOptions<TOptions>>();
            if (options is not null) {
                return options;
            }

            // shoot, no good. let's try get from the configuration
            if (self.TryGetService<IConfiguration>(out var configuration)) {
                var sectionName = typeof(TOptions).Name
                                                  .RemoveTail(Root.Defaults.OptionsSettingsTails);
                var configOptions = configuration.GetSection(sectionName)
                                                 .Get<TOptions>();

                if (configOptions is not null) {
                    return Options.Create(configOptions);
                }
            }

            // whoops...if we reach this far, seems like we don't have
            // the configuration set or missing this particular option.
            // If we have the factory let's construct it. Otherwise,
            // exception it is.
            var factoryOptions = Guard.Against
                                      .Null(factory, nameof(factory))
                                      .Invoke();

            return Options.Create(factoryOptions);
        }

        #endregion
    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Nameless {
    public static class ServiceProviderExtension {
        #region Public Static Methods

        public static TOptions GetPocoOptions<TOptions>(this IServiceProvider self)
            where TOptions : class, new() {
            // let's first check if we have it on our container
            var opts = self.GetService<TOptions>();
            if (opts is not null) { return opts; }

            // ok, we didn't register that option, so let's try find
            // it in our configuration
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

            // if found return; otherwise create it
            return result ?? new();
        }

        public static ILogger GetLogger<T>(this IServiceProvider self) {
            var loggerFactory = self.GetService<ILoggerFactory>();

            return loggerFactory is not null
                ? loggerFactory.CreateLogger<T>()
                : NullLogger<T>.Instance;
        }

        #endregion
    }
}

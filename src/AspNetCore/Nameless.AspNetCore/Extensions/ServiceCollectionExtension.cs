using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.AspNetCore {
    public static class ServiceCollectionExtension {

        #region Public Static Methods

        public static IServiceCollection PushOptions<TOptions>(this IServiceCollection self, IConfiguration configuration, Func<TOptions> optionsProvider) where TOptions : class {
            Prevent.Null(configuration, nameof(configuration));
            Prevent.Null(optionsProvider, nameof(optionsProvider));

            var opts = optionsProvider();
            var key = GetSectionKey<TOptions>();
            configuration.Bind(key, opts);
            self.AddSingleton(opts);
            return self;
        }

        public static IServiceCollection PushOptions<TOptions>(this IServiceCollection self, IConfiguration configuration) where TOptions : class, new()
            => PushOptions(self, configuration, () => new TOptions());

        #endregion

        #region Private Static Methods

        private static string GetSectionKey<TOptions>() {
            return typeof(TOptions).Name
                .Replace("Options", string.Empty)
                .Replace("Settings", string.Empty);
        }

        #endregion
    }
}

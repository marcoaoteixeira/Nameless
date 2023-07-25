using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.AspNetCore {
    public static class ServiceCollectionExtension {
        #region Public Static Methods

        public static IServiceCollection PushOptions<TOptions>(this IServiceCollection self, IConfiguration configuration, Func<TOptions> optionsProvider) where TOptions : class {
            Prevent.Against.Null(configuration, nameof(configuration));
            Prevent.Against.Null(optionsProvider, nameof(optionsProvider));

            var opts = optionsProvider();
            var key = typeof(TOptions).Name.RemoveTail(Internals.ClassTokens.OPTIONS, Internals.ClassTokens.SETTINGS);
            configuration.Bind(key, opts);
            self.AddSingleton(opts);
            return self;
        }

        public static IServiceCollection PushOptions<TOptions>(this IServiceCollection self, IConfiguration configuration) where TOptions : class, new()
            => PushOptions(self, configuration, () => new TOptions());

        #endregion
    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Web {
    public static class ServiceCollectionExtension {
        #region Public Static Methods

        /// <summary>
        /// Registers an object to act like an option that will get its values
        /// from <see cref="IConfiguration"/> (using the Bind method).
        /// </summary>
        /// <typeparam name="TOptions">The type of the object.</typeparam>
        /// <param name="self">The service collection.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="optionsProvider">The option provider.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection PushOptions<TOptions>(this IServiceCollection self, IConfiguration configuration, Func<TOptions> optionsProvider) where TOptions : class {
            Guard.Against.Null(configuration, nameof(configuration));
            Guard.Against.Null(optionsProvider, nameof(optionsProvider));

            var opts = optionsProvider();
            var key = typeof(TOptions).Name.RemoveTail(Root.Defaults.OptionsSettingsTail);
            configuration.Bind(key, opts);
            self.AddSingleton(opts);
            return self;
        }

        /// <summary>
        /// Registers an object to act like an option that will get its values
        /// from <see cref="IConfiguration"/> (using the Bind method).
        /// </summary>
        /// <typeparam name="TOptions">The type of the object.</typeparam>
        /// <param name="self">The service collection.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection PushOptions<TOptions>(this IServiceCollection self, IConfiguration configuration) where TOptions : class, new()
            => PushOptions(self, configuration, () => new TOptions());

        #endregion
    }
}

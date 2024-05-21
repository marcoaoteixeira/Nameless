using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nameless.Infrastructure;
using Nameless.Infrastructure.Impl;

namespace Nameless {
    public static class ServiceCollectionExtension {
        #region Public Static Methods

        /// <summary>
        /// Registers the <see cref="IApplicationContext"/> implementation.
        /// </summary>
        /// <param name="self">The service collection.</param>
        /// <param name="useAppDataSpecialFolder">
        /// <c>true</c> if should use environment special folder or otherwise;
        /// <c>false</c> will use <see cref="AppDomain.CurrentDomain.BaseDirectory"/> + "App_Data"
        /// </param>
        /// <param name="appVersion">The application version.</param>
        /// <returns>
        /// The <paramref name="self"/> instance.
        /// </returns>
        public static IServiceCollection RegisterApplicationContext(this IServiceCollection self, bool useAppDataSpecialFolder = false, Version? appVersion = null)
            => self
                .AddSingleton<IApplicationContext>(provider => {
                    var hostEnvironment = provider.GetRequiredService<IHostEnvironment>();

                    return new ApplicationContext(
                        hostEnvironment,
                        useAppDataSpecialFolder,
                        appVersion ?? new Version(major: 0, minor: 0, build: 0)
                    );
                });

        /// <summary>
        /// Registers an object to act like an option that will get its values
        /// from <see cref="IConfiguration"/> (using the Bind method).
        /// </summary>
        /// <typeparam name="TOptions">The type of the object.</typeparam>
        /// <param name="self">The service collection.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection RegisterPocoOptions<TOptions>(this IServiceCollection self, IConfiguration configuration) where TOptions : class, new()
            => RegisterPocoOptions(self, configuration, () => new TOptions());

        /// <summary>
        /// Registers an object to act like an option that will get its values
        /// from <see cref="IConfiguration"/> (using the Bind method).
        /// </summary>
        /// <typeparam name="TOptions">The type of the object.</typeparam>
        /// <param name="self">The service collection.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="optionsProvider">The option provider.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection RegisterPocoOptions<TOptions>(this IServiceCollection self, IConfiguration configuration, Func<TOptions> optionsProvider) where TOptions : class {
            Guard.Against.Null(configuration, nameof(configuration));
            Guard.Against.Null(optionsProvider, nameof(optionsProvider));

            var opts = optionsProvider();
            var key = typeof(TOptions)
                .Name
                .RemoveTail(Root.Defaults.OptionsSettingsTails);

            configuration.Bind(key, opts);
            self.AddSingleton(opts);

            return self;
        }

        #endregion
    }
}

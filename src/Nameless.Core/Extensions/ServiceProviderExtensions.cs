using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Nameless.Attributes;

namespace Nameless;

public static class ServiceProviderExtensions {
    /// <param name="self">The current <see cref="IServiceProvider" /></param>
    extension(IServiceProvider self) {
        /// <summary>
        ///     Retrieves an instance of <see cref="ILogger{TCategoryName}" />
        ///     from the current <see cref="IServiceProvider" />.
        /// </summary>
        /// <typeparam name="TCategoryName">Type of the logger category.</typeparam>
        /// <returns>
        ///     An instance of <see cref="ILogger{TCategoryName}" />, if <see cref="ILoggerFactory" />
        ///     is available, otherwise; <see cref="NullLogger{T}" />.
        /// </returns>
        public ILogger<TCategoryName> GetLogger<TCategoryName>() {
            var logger = self.GetLoggerCore(typeof(TCategoryName)) as ILogger<TCategoryName>;

            return logger ?? NullLogger<TCategoryName>.Instance;
        }

        /// <summary>
        ///     Retrieves an instance of <see cref="ILogger" />
        ///     from the current <see cref="IServiceProvider" />.
        /// </summary>
        /// <param name="categoryType">Type of the logger category.</param>
        /// <returns>
        ///     An instance of <see cref="ILogger" />, if <see cref="ILoggerFactory" />
        ///     is available, otherwise; <see cref="NullLogger" />.
        /// </returns>
        public ILogger GetLogger(Type categoryType) {
            var logger = self.GetLoggerCore(categoryType);

            return logger ?? NullLogger.Instance;
        }

        /// <summary>
        ///     Retrieves an options instance of the specified type, using the
        ///     service provider, configuration, or a custom factory as
        ///     sources.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///     This method attempts to resolve the options instance in the
        ///     following order: from the service provider, from configuration
        ///     using a section name derived from
        ///     <typeparamref name="TOptions"/>, from the provided factory, or
        ///     by instantiating a new default instance.
        ///     This ensures that an options instance is always returned, even
        ///     if no configuration or registration exists.
        ///     </para>
        ///     <para>
        ///     Note that if <c>AddOptions</c> is registered in the dependency
        ///     injection container, the options instance will always be
        ///     resolved by the provider. If the values within the options
        ///     instance are <see langword="null"/> or empty, this typically
        ///     indicates that no <c>Configure</c> action was applied for that
        ///     options type.
        ///     </para>
        /// </remarks>
        /// <typeparam name="TOptions">
        ///     The type of options to retrieve. Must be a reference type with
        ///     a parameterless constructor.
        /// </typeparam>
        /// <param name="factory">
        ///     An optional factory function used to create the options
        ///     instance if it cannot be obtained from the service provider or
        ///     configuration. If <see langword="null"/>, a new instance is
        ///     created using the parameterless constructor.
        /// </param>
        /// <returns>
        ///     An <see cref="IOptions{TOptions}"/> containing the resolved
        ///     options instance.
        /// </returns>
        public IOptions<TOptions> GetOptions<TOptions>(Func<TOptions>? factory = null)
            where TOptions : class, new() {
            // 1st attempt: from the service provider;
            var fromProvider = self.GetService<IOptions<TOptions>>();
            if (fromProvider is not null) {
                return fromProvider;
            }

            // 2nd attempt: from the configuration
            var sectionName = ConfigurationSectionNameAttribute.GetSectionName<TOptions>();
            var configuration = self.GetService<IConfiguration>();
            var fromConfiguration = configuration?.GetSection(sectionName).Get<TOptions>();
            if (fromConfiguration is not null) {
                return Options.Create(fromConfiguration);
            }

            // 3rd attempt: from the factory
            var fromFactory = factory?.Invoke();
            if (fromFactory is not null) {
                return Options.Create(fromFactory);
            }

            // lastly, create from the parameterless constructor
            return Options.Create(new TOptions());
        }

        private ILogger? GetLoggerCore(Type type) {
            return self
                   .GetService<ILoggerFactory>()?
                   .CreateLogger(type.FullName ?? type.Name);
        }
    }
}
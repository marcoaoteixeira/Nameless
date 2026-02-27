using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Nameless.Attributes;

namespace Nameless.Autofac;

/// <summary>
///     <see cref="IComponentContext" /> extension methods.
/// </summary>
public static class ComponentContextExtensions {
    /// <param name="self">
    ///     The current <see cref="IComponentContext" />.
    /// </param>
    extension(IComponentContext self) {
        /// <summary>
        ///     Retrieves an instance of <see cref="ILogger{TCategoryName}" />
        ///     from the current <see cref="IComponentContext" />.
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
        ///     from the current <see cref="IComponentContext" />.
        /// </summary>
        /// <param name="categoryType">Type of the logger category.</param>
        /// <returns>
        ///     An instance of <see cref="ILogger" />, if <see cref="ILoggerFactory" />
        ///     is available, otherwise; <see cref="NullLogger" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     if <paramref name="categoryType" /> is <see langword="null"/>.
        /// </exception>
        public ILogger GetLogger(Type categoryType) {
            var logger = self.GetLoggerCore(categoryType);

            return logger ?? NullLogger.Instance;
        }

        public IOptions<TOptions> GetOptions<TOptions>(Func<TOptions>? optionsFactory = null)
            where TOptions : class, new() {
            // 1st attempt: retrieve from the provider.
            self.TryResolve<IOptions<TOptions>>(out var fromProvider);
            if (fromProvider is not null) {
                return fromProvider;
            }

            // 2nd attempt: retrieve from configuration
            self.TryResolve<IConfiguration>(out var configuration);
            var sectionName = ConfigurationSectionNameAttribute.GetSectionName<TOptions>();
            var fromConfiguration = configuration?.GetSection(sectionName)?.Get<TOptions>();
            if (fromConfiguration is not null) {
                return Options.Create(fromConfiguration);
            }

            // 3rd attempt: retrieve it from the factory function
            var fromFactory = optionsFactory?.Invoke();
            if (fromFactory is not null) {
                return Options.Create(fromFactory);
            }

            // lastly, if everything failed, then construct the object
            return Options.Create(new TOptions());
        }

        private ILogger? GetLoggerCore(Type type) {
            return self
                   .ResolveOptional<ILoggerFactory>()?
                   .CreateLogger(type.FullName ?? type.Name);
        }
    }
}
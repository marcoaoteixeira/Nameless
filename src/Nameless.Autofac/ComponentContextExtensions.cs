using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

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
            return self.GetLoggerCore(typeof(TCategoryName)) as ILogger<TCategoryName>
                   ?? NullLogger<TCategoryName>.Instance;
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
            return self.GetLoggerCore(categoryType)
                   ?? NullLogger.Instance;
        }

        /// <summary>
        ///     Retrieves an <see cref="IOptions{TOptions}" /> from the current <see cref="IComponentContext" />.
        /// </summary>
        /// <typeparam name="TOptions">Type of the options.</typeparam>
        /// <returns>
        ///     An instance of <see cref="IOptions{TOptions}" />.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     if <paramref name="self" /> is <see langword="null"/>.
        /// </exception>
        public IOptions<TOptions> GetOptions<TOptions>()
            where TOptions : class, new() {
            return self.GetOptions(() => new TOptions());
        }

        /// <summary>
        ///     Retrieves an <see cref="IOptions{TOptions}" /> from the current
        ///     <see cref="IComponentContext" />.
        /// </summary>
        /// <typeparam name="TOptions">
        ///     Type of the options.
        /// </typeparam>
        /// <param name="fallback">
        ///     An options fallback, if the options were not to be found.
        /// </param>
        /// <returns>
        ///     An instance of <see cref="IOptions{TOptions}" />.
        /// </returns>
        public IOptions<TOptions> GetOptions<TOptions>(Func<TOptions> fallback)
            where TOptions : class {
            // let's first check if our provider can resolve this option
            if (self.TryResolve<IOptions<TOptions>>(out var options)) {
                return options;
            }

            // shoot, no good. let's try get from the configuration
            if (self.TryResolve<IConfiguration>(out var configuration)) {
                var sectionName = typeof(TOptions).Name;
                var optionsFromConfiguration = configuration.GetSection(sectionName)
                                                            .Get<TOptions>();

                if (optionsFromConfiguration is not null) {
                    return Options.Create(optionsFromConfiguration);
                }
            }

            // whoops...if we reach this far, seems like we don't have
            // the configuration set or missing this particular option.
            // If we have the fallback let's construct it.
            var optionsFromFactory = fallback();

            return Options.Create(optionsFromFactory);
        }

        private ILogger? GetLoggerCore(Type type) {
            return self
                   .ResolveOptional<ILoggerFactory>()?
                   .CreateLogger(type.FullName ?? type.Name);
        }
    }
}
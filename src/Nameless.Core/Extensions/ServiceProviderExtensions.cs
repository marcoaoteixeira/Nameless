using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Nameless;

public static class ServiceProviderExtensions {
    /// <summary>
    ///     Retrieves an instance of <see cref="ILogger{TCategoryName}" />
    ///     from the current <see cref="IServiceProvider" />.
    /// </summary>
    /// <typeparam name="TCategoryName">Type of the logger category.</typeparam>
    /// <param name="self">The current <see cref="IServiceProvider" /></param>
    /// <returns>
    ///     An instance of <see cref="ILogger{TCategoryName}" />, if <see cref="ILoggerFactory" />
    ///     is available, otherwise; <see cref="NullLogger{T}" />.
    /// </returns>
    public static ILogger<TCategoryName> GetLogger<TCategoryName>(this IServiceProvider self) {
        var loggerFactory = self.GetService<ILoggerFactory>();

        return loggerFactory is not null
            ? loggerFactory.CreateLogger<TCategoryName>()
            : NullLogger<TCategoryName>.Instance;
    }

    /// <summary>
    ///     Retrieves an instance of <see cref="ILogger" />
    ///     from the current <see cref="IServiceProvider" />.
    /// </summary>
    /// <param name="self">The current <see cref="IServiceProvider" /></param>
    /// <param name="categoryType">Type of the logger category.</param>
    /// <returns>
    ///     An instance of <see cref="ILogger" />, if <see cref="ILoggerFactory" />
    ///     is available, otherwise; <see cref="NullLogger" />.
    /// </returns>
    public static ILogger GetLogger(this IServiceProvider self, Type categoryType) {
        var loggerFactory = self.GetService<ILoggerFactory>();

        return loggerFactory is not null
            ? loggerFactory.CreateLogger(categoryType)
            : NullLogger.Instance;
    }

    public static ILogger GetLogger(this IServiceProvider self, string categoryName) {
        var loggerFactory = self.GetService<ILoggerFactory>();

        return loggerFactory is not null
            ? loggerFactory.CreateLogger(categoryName)
            : NullLogger.Instance;
    }

    /// <summary>
    ///     Retrieves an <see cref="IOptions{TOptions}" /> from the current <see cref="IServiceProvider" />.
    /// </summary>
    /// <typeparam name="TOptions">Type of the options.</typeparam>
    /// <param name="self">The current <see cref="IServiceProvider" /></param>
    /// <returns>
    ///     An instance of <see cref="IOptions{TOptions}" />.
    /// </returns>
    public static IOptions<TOptions> GetOptions<TOptions>(this IServiceProvider self)
        where TOptions : class, new() {
        return self.GetOptions(() => new TOptions());
    }

    /// <summary>
    ///     Retrieves an <see cref="IOptions{TOptions}" /> from the current <see cref="IServiceProvider" />.
    /// </summary>
    /// <typeparam name="TOptions">Type of the options.</typeparam>
    /// <param name="self">The current <see cref="IServiceProvider" /></param>
    /// <param name="optionsFactory">An options factory, if the options were not to be found.</param>
    /// <returns>
    ///     An instance of <see cref="IOptions{TOptions}" />.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="self" /> or
    ///     <paramref name="optionsFactory" /> is <see langword="null"/>.
    /// </exception>
    public static IOptions<TOptions> GetOptions<TOptions>(this IServiceProvider self, Func<TOptions> optionsFactory)
        where TOptions : class {
        // let's first check if our provider can resolve this option
        var options = self.GetService<IOptions<TOptions>>();
        if (options is not null) {
            return options;
        }

        // shoot, no good. let's try get it from configuration service
        var configuration = self.GetService<IConfiguration>();
        if (configuration is not null) {
            var sectionName = typeof(TOptions).Name;
            var configOptions = configuration.GetSection(sectionName)
                                             .Get<TOptions>();

            if (configOptions is not null) {
                return Options.Create(configOptions);
            }
        }

        // whoops...if we reach this far, seems like we don't have
        // the configuration set or missing this particular option.
        // If we have the optionsFactory let's construct it.
        var factoryOptions = optionsFactory.Invoke();

        return Options.Create(factoryOptions);
    }
}
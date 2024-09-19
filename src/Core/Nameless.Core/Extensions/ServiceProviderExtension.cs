using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Nameless;

public static class ServiceProviderExtension {
    /// <summary>
    /// Tries to retrieve the specified service.
    /// </summary>
    /// <typeparam name="TService">Type of the service.</typeparam>
    /// <param name="self">The current <see cref="IServiceProvider"/>.</param>
    /// <param name="service">The service, if found.</param>
    /// <returns>
    /// <c>true</c> if the service was found, otherwise; <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> is <c>null</c>.
    /// </exception>
    public static bool TryGetService<TService>(this IServiceProvider self, [NotNullWhen(returnValue: true)] out TService? service) {
        Prevent.Argument.Null(self);

        service = self.GetService<TService>();

        return service is not null;
    }

    /// <summary>
    /// Tries to retrieve the specified service by its key.
    /// </summary>
    /// <typeparam name="TService">Type of the service.</typeparam>
    /// <param name="self">The current <see cref="IServiceProvider"/>.</param>
    /// <param name="key">The service key.</param>
    /// <param name="service">The service, if found.</param>
    /// <returns>
    /// <c>true</c> if the service was found, otherwise; <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> is <c>null</c>.
    /// </exception>
    public static bool TryGetKeyedService<TService>(this IServiceProvider self, string key, [NotNullWhen(returnValue: true)] out TService? service) {
        Prevent.Argument.Null(self);

        service = self.GetKeyedService<TService>(key);

        return service is not null;
    }

    /// <summary>
    /// Retrieves an instance of <see cref="ILogger{TCategoryName}"/>
    /// from the current <see cref="IServiceProvider"/>.
    /// </summary>
    /// <typeparam name="TCategoryName">Type of the logger category.</typeparam>
    /// <param name="self">The current <see cref="IServiceProvider"/></param>
    /// <returns>
    /// An instance of <see cref="ILogger{TCategoryName}"/>, if <see cref="ILoggerFactory"/>
    /// is available, otherwise; <see cref="NullLogger{T}"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> is <c>null</c>.
    /// </exception>
    public static ILogger<TCategoryName> GetLogger<TCategoryName>(this IServiceProvider self) {
        Prevent.Argument.Null(self);

        var loggerFactory = self.GetService<ILoggerFactory>();

        return loggerFactory is not null
            ? loggerFactory.CreateLogger<TCategoryName>()
            : NullLogger<TCategoryName>.Instance;
    }

    /// <summary>
    /// Retrieves an instance of <see cref="ILogger"/>
    /// from the current <see cref="IServiceProvider"/>.
    /// </summary>
    /// <param name="self">The current <see cref="IServiceProvider"/></param>
    /// <param name="categoryType">Type of the logger category.</param>
    /// <returns>
    /// An instance of <see cref="ILogger"/>, if <see cref="ILoggerFactory"/>
    /// is available, otherwise; <see cref="NullLogger"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> is <c>null</c>.
    /// </exception>
    public static ILogger GetLogger(this IServiceProvider self, Type categoryType) {
        Prevent.Argument.Null(self);

        var loggerFactory = self.GetService<ILoggerFactory>();

        return loggerFactory is not null
            ? loggerFactory.CreateLogger(categoryType)
            : NullLogger.Instance;
    }

    /// <summary>
    /// Retrieves an <see cref="IOptions{TOptions}"/> from the current <see cref="IServiceProvider"/>.
    /// </summary>
    /// <typeparam name="TOptions">Type of the options.</typeparam>
    /// <param name="self">The current <see cref="IServiceProvider"/></param>
    /// <returns>
    /// An instance of <see cref="IOptions{TOptions}"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> is <c>null</c>.
    /// </exception>
    public static IOptions<TOptions> GetOptions<TOptions>(this IServiceProvider self)
        where TOptions : class, new()
        => GetOptions(self, () => new TOptions());

    /// <summary>
    /// Retrieves an <see cref="IOptions{TOptions}"/> from the current <see cref="IServiceProvider"/>.
    /// </summary>
    /// <typeparam name="TOptions">Type of the options.</typeparam>
    /// <param name="self">The current <see cref="IServiceProvider"/></param>
    /// <param name="optionsFactory">An options factory, if the options were not to be found.</param>
    /// <returns>
    /// An instance of <see cref="IOptions{TOptions}"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="self"/> or
    /// <paramref name="optionsFactory"/> is <c>null</c>.
    /// </exception>
    public static IOptions<TOptions> GetOptions<TOptions>(this IServiceProvider self, Func<TOptions> optionsFactory)
        where TOptions : class {
        Prevent.Argument.Null(self);
        Prevent.Argument.Null(optionsFactory);

        // let's first check if our provider can resolve this option
        var options = self.GetService<IOptions<TOptions>>();
        if (options is not null) {
            return options;
        }

        // shoot, no good. let's try get from the configuration
        if (self.TryGetService<IConfiguration>(out var configuration)) {
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
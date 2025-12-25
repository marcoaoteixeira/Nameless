using System.Globalization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.Localization.Json.Infrastructure;
using Nameless.Localization.Json.Internals;
using Nameless.Localization.Json.Objects;

namespace Nameless.Localization.Json;

/// <summary>
/// Implements a factory for creating instances of <see cref="IStringLocalizer"/> that retrieves localized strings from JSON resource files.
/// </summary>
public sealed class StringLocalizerFactory : IStringLocalizerFactory {
    private readonly ICultureProvider _cultureContext;
    private readonly IResourceManager _resourceManager;
    private readonly IOptions<JsonLocalizationOptions> _options;

    private readonly ILogger<StringLocalizerFactory> _stringLocalizerFactoryLogger;
    private readonly ILogger<StringLocalizer> _stringLocalizerLogger;

    /// <summary>
    /// Initializes a new instance of the <see cref="StringLocalizerFactory"/> class.
    /// </summary>
    /// <param name="cultureContext">The culture context.</param>
    /// <param name="resourceManager">The resource manager.</param>
    /// <param name="options">The localization options.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="cultureContext"/> or
    ///     <paramref name="resourceManager"/> or
    ///     <paramref name="options"/> or
    ///     <paramref name="loggerFactory"/> is <see langword="null"/>.
    /// </exception>
    public StringLocalizerFactory(ICultureProvider cultureContext,
                                  IResourceManager resourceManager,
                                  IOptions<JsonLocalizationOptions> options,
                                  ILoggerFactory loggerFactory) {
        _cultureContext = cultureContext;
        _resourceManager = resourceManager;
        _options = options;

        _stringLocalizerFactoryLogger = loggerFactory.CreateLogger<StringLocalizerFactory>();
        _stringLocalizerLogger = loggerFactory.CreateLogger<StringLocalizer>();
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="resourceSource"/> is <see langword="null"/>.
    /// </exception>
    public IStringLocalizer Create(Type resourceSource) {
        var baseName = resourceSource.Namespace ?? string.Empty;
        var location = _options.Value.RemoveArityFromGenerics
            ? resourceSource.GetNameWithoutArity()
            : resourceSource.Name;

        return Create(baseName, location);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="baseName"/> or
    ///     <paramref name="location"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Thrown when <paramref name="baseName"/> or
    ///     <paramref name="location"/> is empty or white spaces.
    /// </exception>
    public IStringLocalizer Create(string baseName, string location) {
        return GetLocalizer(baseName, location, _cultureContext.GetCurrentCulture());
    }

    private Resource GetResource(string baseName, string location, CultureInfo culture) {
        var resourceName = $"[{baseName}] {location}";

        _stringLocalizerFactoryLogger.GettingResource(resourceName);

        var resource = _resourceManager.GetResource(baseName, location, culture.Name);

        _stringLocalizerFactoryLogger
           .OnCondition(!resource.IsAvailable)
           .ResourceNotAvailable(resourceName);

        return resource;
    }

    private StringLocalizer GetLocalizer(string baseName, string location, CultureInfo culture) {
        var resource = GetResource(baseName, location, culture);

        return new StringLocalizer(baseName,
            location,
            culture,
            resource,
            GetLocalizer,
            _stringLocalizerLogger);
    }
}
using System.Globalization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.Localization.Json.Infrastructure;
using Nameless.Localization.Json.Objects;
using Nameless.Localization.Json.Options;

namespace Nameless.Localization.Json;

public sealed class StringLocalizerFactory : IStringLocalizerFactory {
    private readonly ICultureProvider _cultureContext;
    private readonly IResourceManager _resourceManager;
    private readonly IOptions<LocalizationOptions> _options;
    private readonly ILogger<StringLocalizerFactory> _logger;
    private readonly ILogger<StringLocalizer> _loggerForStringLocalizer;

    public StringLocalizerFactory(ICultureProvider cultureContext,
                                  IResourceManager resourceManager,
                                  IOptions<LocalizationOptions> options,
                                  ILogger<StringLocalizerFactory> logger,
                                  ILogger<StringLocalizer> loggerForStringLocalizer) {
        _cultureContext = Prevent.Argument.Null(cultureContext);
        _resourceManager = Prevent.Argument.Null(resourceManager);
        _options = Prevent.Argument.Null(options);
        _logger = Prevent.Argument.Null(logger);
        _loggerForStringLocalizer = Prevent.Argument.Null(loggerForStringLocalizer);
    }

    public IStringLocalizer Create(Type resourceSource) {
        Prevent.Argument.Null(resourceSource);

        var baseName = Prevent.Argument.NullOrWhiteSpace(resourceSource.Namespace);
        var location = _options.Value.RemoveArityFromGenerics
            ? resourceSource.GetNameWithoutArity()
            : resourceSource.Name;

        return Create(baseName, location);
    }

    public IStringLocalizer Create(string baseName, string location)
        => GetLocalizer(baseName: Prevent.Argument.NullOrWhiteSpace(baseName),
                        location: Prevent.Argument.NullOrWhiteSpace(location),
                        culture: _cultureContext.GetCurrentCulture());

    private Resource GetResource(string baseName, string location, CultureInfo culture) {
        var resourceName = $"[{baseName}] {location}";

        _logger.GettingResource(resourceName);

        var resource = _resourceManager.GetResource(baseName, location, culture.Name);

        _logger.OnCondition(!resource.IsAvailable)
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
                                   _loggerForStringLocalizer);
    }
}
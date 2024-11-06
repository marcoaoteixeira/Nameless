using System.Globalization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Nameless.Localization.Json.Infrastructure;
using Nameless.Localization.Json.Objects;

namespace Nameless.Localization.Json;

public sealed class StringLocalizerFactory : IStringLocalizerFactory {
    private readonly ICultureContext _cultureContext;
    private readonly ITranslationManager _translationManager;
    private readonly ILogger<StringLocalizerFactory> _logger;
    private readonly ILogger<StringLocalizer> _loggerForStringLocalizer;

    public StringLocalizerFactory(ICultureContext cultureContext,
                                  ITranslationManager translationManager,
                                  ILogger<StringLocalizerFactory> logger,
                                  ILogger<StringLocalizer> loggerForStringLocalizer) {
        _cultureContext = Prevent.Argument.Null(cultureContext);
        _translationManager = Prevent.Argument.Null(translationManager);
        _logger = Prevent.Argument.Null(logger);
        _loggerForStringLocalizer = Prevent.Argument.Null(loggerForStringLocalizer);
    }

    public IStringLocalizer Create(Type resourceSource)
        => GetLocalizer(_cultureContext.GetCurrentCulture(), resourceSource.Assembly.GetName().Name!, resourceSource.FullName!);

    public IStringLocalizer Create(string baseName, string location)
        => GetLocalizer(_cultureContext.GetCurrentCulture(), baseName, location);

    private Region GetRegion(CultureInfo culture, string resourceName, string resourcePath) {
        var key = $"[{resourceName}] {resourcePath}";

        _logger.GettingRegionFromTranslationObject(key);

        var translation = _translationManager.GetTranslation(culture.Name);
        var regionFound = translation.TryGetRegion(key, out var region);

        _logger.OnCondition(!regionFound)
               .RegionNotFoundInTranslationObject(key);

        return region ?? new Region(name: key, messages: []);
    }

    private StringLocalizer GetLocalizer(CultureInfo culture, string resourceName, string resourcePath) {
        var region = GetRegion(culture, resourceName, resourcePath);

        return new StringLocalizer(culture,
                                   resourceName,
                                   resourcePath,
                                   region,
                                   GetLocalizer,
                                   _loggerForStringLocalizer);
    }
}
using System.Globalization;
using Microsoft.Extensions.Localization;
using Nameless.Localization.Json.Infrastructure;
using Nameless.Localization.Json.Objects;

namespace Nameless.Localization.Json;

public sealed class StringLocalizerFactory : IStringLocalizerFactory {
    private readonly ICultureContext _cultureContext;
    private readonly ITranslationManager _translationManager;

    public StringLocalizerFactory(ICultureContext cultureContext, ITranslationManager translationManager) {
        _cultureContext = Prevent.Argument.Null(cultureContext);
        _translationManager = Prevent.Argument.Null(translationManager);
    }

    public IStringLocalizer Create(Type resourceSource)
        => GetLocalizer(_cultureContext.GetCurrentCulture(), resourceSource.Assembly.GetName().Name!, resourceSource.FullName!);

    public IStringLocalizer Create(string baseName, string location)
        => GetLocalizer(_cultureContext.GetCurrentCulture(), baseName, location);

    private Region GetRegion(CultureInfo culture, string resourceName, string resourcePath) {
        var key = $"[{resourceName}] {resourcePath}";
        var translation = _translationManager.GetTranslation(culture.Name);

        return translation.TryGetRegion(key, out var region)
            ? region
            : new Region(name: key, messages: []);
    }

    private StringLocalizer GetLocalizer(CultureInfo culture, string resourceName, string resourcePath) {
        var region = GetRegion(culture, resourceName, resourcePath);

        return new StringLocalizer(culture, resourceName, resourcePath, region, GetLocalizer);
    }
}
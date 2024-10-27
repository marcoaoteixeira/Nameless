using System.Globalization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Nameless.Localization.Json.Objects;

namespace Nameless.Localization.Json;

public sealed class StringLocalizer : IStringLocalizer {
    private readonly CultureInfo _culture;
    private readonly string _resourceName;
    private readonly string _resourcePath;
    private readonly Region _region;
    private readonly Func<CultureInfo, string, string, IStringLocalizer> _factory;
    private readonly ILogger<StringLocalizer> _logger;

    public string Location => $"{_culture}::{_resourceName}::{_resourcePath}";

    public StringLocalizer(CultureInfo culture,
                           string resourceName,
                           string resourcePath,
                           Region region,
                           Func<CultureInfo, string, string, IStringLocalizer> factory,
                           ILogger<StringLocalizer> logger) {
        _culture = Prevent.Argument.Null(culture);
        _resourceName = Prevent.Argument.NullOrWhiteSpace(resourceName);
        _resourcePath = Prevent.Argument.NullOrWhiteSpace(resourcePath);
        _region = Prevent.Argument.Null(region);
        _factory = Prevent.Argument.Null(factory);
        _logger = Prevent.Argument.Null(logger);
    }

    public LocalizedString this[string name]
        => GetLocalizedString(name);

    public LocalizedString this[string name, params object[] arguments]
        => GetLocalizedString(name, arguments);

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) {
        foreach (var entry in _region.Messages) {
            yield return new LocalizedString(entry.ID, entry.Text, false, Location);
        }

        if (!includeParentCultures) {
            yield break;
        }

        foreach (var culture in _culture.GetParents().Skip(1)) {
            var localizer = _factory(culture, _resourceName, _resourcePath);
            foreach (var localeString in localizer.GetAllStrings(includeParentCultures: false)) {
                yield return localeString;
            }
        }
    }

    private LocalizedString GetLocalizedString(string text, params object[] args) {
        var found = _region.TryGetMessage(text, out var message);

        _logger.OnCondition(!found)
               .TranslationNotFoundForMessageWithID(text);

        var (name, value) = message is not null
            ? (message.ID, message.Text)
            : (text, text);

        return new LocalizedString(name: args.Length > 0 ? string.Format(name, args) : name,
                                   value: args.Length > 0 ? string.Format(value, args) : value,
                                   resourceNotFound: !found,
                                   searchedLocation: Location);
    }
}
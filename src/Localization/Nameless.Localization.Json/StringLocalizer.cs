using System.Globalization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Nameless.Localization.Json.Objects;

namespace Nameless.Localization.Json;

public sealed class StringLocalizer : IStringLocalizer {
    private readonly string _baseName;
    private readonly string _location;
    private readonly CultureInfo _culture;
    private readonly Resource _resource;
    private readonly Func<string, string, CultureInfo, IStringLocalizer> _factory;
    private readonly ILogger<StringLocalizer> _logger;

    public string Location => $"{_baseName}::{_location}::{_culture}";

    public StringLocalizer(string baseName,
                           string location,
                           CultureInfo culture,
                           Resource resource,
                           Func<string, string, CultureInfo, IStringLocalizer> factory,
                           ILogger<StringLocalizer> logger) {
        _baseName = Prevent.Argument.NullOrWhiteSpace(baseName);
        _location = Prevent.Argument.NullOrWhiteSpace(location);
        _culture = Prevent.Argument.Null(culture);
        _resource = Prevent.Argument.Null(resource);
        _factory = Prevent.Argument.Null(factory);
        _logger = Prevent.Argument.Null(logger);
    }

    public LocalizedString this[string name]
        => GetLocalizedString(name);

    public LocalizedString this[string name, params object[] arguments]
        => GetLocalizedString(name, arguments);

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) {
        foreach (var entry in _resource.Messages) {
            yield return new LocalizedString(entry.Id, entry.Text, false, Location);
        }

        if (!includeParentCultures) {
            yield break;
        }

        var cultures = _culture.GetParents()
                               .Skip(count: 1) // Skips the first one since we get all messages already (above)
                               .Append(CultureInfo.InvariantCulture); // Appends the invariant culture as last resort.
        foreach (var culture in cultures) {
            var localizer = _factory(_baseName, _location, culture);
            foreach (var localeString in localizer.GetAllStrings(includeParentCultures: false)) {
                yield return localeString;
            }
        }
    }

    private LocalizedString GetLocalizedString(string text, params object[] args) {
        var found = _resource.TryGetMessage(text, out var message);

        _logger.OnCondition(!found).MessageNotFound(text);

        var (name, value) = message is not null
            ? (message.Id, message.Text)
            : (text, text);

        return new LocalizedString(name: args.Length > 0 ? string.Format(name, args) : name,
                                   value: args.Length > 0 ? string.Format(value, args) : value,
                                   resourceNotFound: !found,
                                   searchedLocation: Location);
    }
}
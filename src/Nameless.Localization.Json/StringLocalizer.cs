using System.Globalization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Nameless.Localization.Json.Objects;

namespace Nameless.Localization.Json;

/// <summary>
/// Implements a string localizer that retrieves localized strings from a JSON resource file.
/// </summary>
public sealed class StringLocalizer : IStringLocalizer {
    private readonly string _baseName;
    private readonly CultureInfo _culture;
    private readonly Func<string, string, CultureInfo, IStringLocalizer> _factory;
    private readonly string _location;
    private readonly ILogger<StringLocalizer> _logger;
    private readonly Resource _resource;

    /// <summary>
    /// Gets the location of the resource, which is a combination of the base name, location, and culture.
    /// </summary>
    public string Location => $"{_baseName}::{_location}::{_culture}";

    /// <summary>
    /// Initializes a new instance of the <see cref="StringLocalizer"/> class.
    /// </summary>
    /// <param name="baseName">The base name.</param>
    /// <param name="location">The location.</param>
    /// <param name="culture">The culture.</param>
    /// <param name="resource">The resource.</param>
    /// <param name="factory">The factory responsible for create the <see cref="StringLocalizer"/> for the culture tree.</param>
    /// <param name="logger">The logger.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="baseName"/> or
    ///     <paramref name="location"/> or
    ///     <paramref name="culture"/> or
    ///     <paramref name="resource"/> or
    ///     <paramref name="factory"/> or
    ///     <paramref name="logger"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Thrown when <paramref name="baseName"/> or
    ///     <paramref name="location"/> is empty or white spaces.
    /// </exception>
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

    /// <inheritdoc />
    public LocalizedString this[string name]
        => GetLocalizedString(name);

    /// <inheritdoc />
    public LocalizedString this[string name, params object[] arguments]
        => GetLocalizedString(name, arguments);

    /// <inheritdoc />
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) {
        foreach (var entry in _resource.Messages) {
            yield return new LocalizedString(entry.Id, entry.Text, false, Location);
        }

        if (!includeParentCultures) {
            yield break;
        }

        var cultures = _culture.GetParents()
                               .Skip(1) // Skips the first one since we get all messages already (above)
                               .Append(CultureInfo.InvariantCulture); // Appends the invariant culture as last resort.
        foreach (var culture in cultures) {
            var localizer = _factory(_baseName, _location, culture);
            foreach (var localeString in localizer.GetAllStrings(false)) {
                yield return localeString;
            }
        }
    }

    private LocalizedString GetLocalizedString(string text, params object[] args) {
        var found = _resource.TryGetMessage(text, out var message);

        message ??= new Message(text, text);

        _logger.OnCondition(!found).MessageNotFound(text);

        return new LocalizedString(message.GetId(args),
            message.GetText(args),
            !found,
            Location);
    }
}
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Nameless.Contracts;
using Nameless.Windows.Localization.Json;

namespace Nameless.Windows.Localization;

/// <summary>
///     <see cref="ITranslator"/> implementation backed by one flat JSON
///     dictionary file per culture (<c>{ "Key1": "Text1" }</c>), read
///     through <see cref="ITranslationFileProvider"/>. Supports runtime
///     culture switching: calling <see cref="SetCulture(CultureInfo)"/>
///     raises <see cref="INotifyPropertyChanged.PropertyChanged"/> with
///     <c>"Item[]"</c> so all WPF indexer bindings re-evaluate
///     automatically.
/// </summary>
/// <remarks>
///     Culture resolution falls back from the specific culture (e.g.
///     <c>pt-BR</c>) to its neutral culture (<c>pt</c>) and finally to the
///     culture-less <c>default</c> file. Use
///     <see cref="TranslatorExtension"/> in XAML to bind to this localizer.
/// </remarks>
public sealed class JsonTranslator : ITranslator {
    // Cached event args — avoids allocating on every culture change
    private static readonly PropertyChangedEventArgs IndexerArgs = new("Item[]");

    private readonly ITranslationFileProvider _fileProvider;
    private readonly ILogger<JsonTranslator> _logger;

    private CultureInfo _currentCulture;
    private IReadOnlyDictionary<string, string>? _translations;

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public CultureInfo CurrentCulture {
        get => _currentCulture;
        private set => SetField(ref _currentCulture, value);
    }

    /// <inheritdoc />
    public string this[string key] => this[key, parameters: []];

    /// <inheritdoc />
    public string this[string key, params object[] parameters] {
        get {
            if (string.IsNullOrWhiteSpace(key)) {
                return string.Empty;
            }

            var translations = GetTranslations();
            if (translations.TryGetValue(key, out var template) && !string.IsNullOrWhiteSpace(template)) {
                return string.Format(template, parameters);
            }

            CommonLog.Debug(_logger, message: $"Missing key '{key}'", GetType().Tag);

            return $"[{key}]";
        }
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="JsonTranslator"/> class.
    /// </summary>
    /// <param name="fileProvider">
    ///     The provider used to read each culture's JSON translation file.
    /// </param>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    public JsonTranslator(ITranslationFileProvider fileProvider, ILogger<JsonTranslator> logger) {
        _fileProvider = Throws.When.Null(fileProvider);
        _logger = Throws.When.Null(logger);
        _currentCulture = CultureInfo.CurrentUICulture;
    }

    /// <inheritdoc />
    public void SetCulture(CultureInfo culture) {
        if (Equals(_currentCulture, culture)) {
            return;
        }

        // Invalidates the cached dictionary — the next lookup resolves
        // the fallback chain again, now against the new culture.
        _translations = null;

        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        CurrentCulture = culture; // triggers "CurrentCulture" property notification

        // Invalidates ALL indexer bindings — forces every {Binding [Key]} to re-evaluate
        PropertyChanged?.Invoke(this, IndexerArgs);
    }

    /// <inheritdoc />
    public void SetCulture(string cultureName) {
        SetCulture(CultureInfo.GetCultureInfo(cultureName));
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null) {
        if (EqualityComparer<T>.Default.Equals(field, value)) { return; }

        field = value;

        OnPropertyChanged(propertyName);
    }

    private IReadOnlyDictionary<string, string> GetTranslations() {
        return _translations ??= LoadTranslations();
    }

    private IReadOnlyDictionary<string, string> LoadTranslations() {
        // Merges per-key, not per-file: loading in reverse fallback order
        // (default first, then neutral, then specific) means a more
        // specific file's entries overwrite less specific ones only for
        // the keys it actually defines — a key missing from, say, pt-BR
        // still resolves from pt or default instead of rendering "[key]".
        // This matches ResourceManager's own per-key resolution.
        var merged = new Dictionary<string, string>();

        foreach (var key in GetCultureFallbackChain(_currentCulture).Reverse()) {
            if (!_fileProvider.TryRead(key, out var json)) {
                continue;
            }

            Dictionary<string, string>? parsed;
            try {
                parsed = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            }
            catch (JsonException exception) {
                CommonLog.JsonDeserializationFailure(_logger, typeof(Dictionary<string, string>), exception, tag: key);
                continue;
            }

            if (parsed is null) {
                continue;
            }

            foreach (var (translationKey, translationValue) in parsed) {
                merged[translationKey] = translationValue;
            }
        }

        return merged;
    }

    /// <summary>
    ///     Builds the lookup order for a culture: the specific culture
    ///     name, its neutral ancestors, and finally the literal
    ///     <c>"default"</c>.
    /// </summary>
    private static IEnumerable<string> GetCultureFallbackChain(CultureInfo culture) {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var current = culture;
        while (!string.IsNullOrEmpty(current.Name)) {
            if (seen.Add(current.Name)) {
                yield return current.Name;
            }

            current = current.Parent;
        }

        yield return "default";
    }
}

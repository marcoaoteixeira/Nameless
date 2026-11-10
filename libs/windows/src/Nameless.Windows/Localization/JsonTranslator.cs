using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Nameless.Contracts;
using Nameless.Windows.Localization.Json;

namespace Nameless.Windows.Localization;

/// <summary>
///     <see cref="ITranslator"/> implementation backed by one JSON file per
///     culture, read through <see cref="ITranslationFileProvider"/>. On
///     disk, keys are grouped by section purely for human organization
///     (<c>{ "SectionName": [ { "Key1": "Text1" } ] }</c>) — the section
///     name is never part of the lookup key, so <c>this["Key1"]</c> works
///     exactly as it would against a flat file. Supports runtime
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
///     <para>
///         Subscribes to <see cref="ITranslationFileProvider.Changed"/>
///         (#191) to support hot-reload: when the provider reports that a
///         culture's backing file changed on disk, only that file is
///         re-read and re-parsed in full. A successful reload replaces
///         that file's cached entries and re-raises the "Item[]"
///         <see cref="PropertyChanged"/> signal; a failed one (parse
///         error, shape mismatch, missing file) is logged and leaves the
///         last known-good state — and the UI — exactly as it was,
///         without the refresh signal firing.
///     </para>
/// </remarks>
public sealed class JsonTranslator : ITranslator, IDisposable {
    // Cached event args — avoids allocating on every culture change
    private static readonly PropertyChangedEventArgs IndexerArgs = new("Item[]");

    // Default JsonSerializerOptions silently keep the *last* value for a
    // duplicate property — which would hide a duplicate key within the
    // same entry object, and silently drop an entire duplicate top-level
    // section, instead of failing loudly like every other shape problem
    // this type logs. Disallowing duplicates turns both into a JsonException,
    // which already routes through the existing "log and keep last
    // known-good state" path below.
    private static readonly JsonSerializerOptions ParseOptions = new() { AllowDuplicateProperties = false };

    private readonly ITranslationFileProvider _fileProvider;
    private readonly ILogger<JsonTranslator> _logger;

    // Guards every read/write of _fileCache, _translations and
    // _currentCulture — OnFileChanged runs on whatever thread the
    // provider raises Changed from (a ThreadPool timer, one per watched
    // file, in FileSystemTranslationFileProvider's case), so two files
    // changing together (e.g. a git pull or bulk save) can run
    // concurrently with each other and with a UI-thread lookup.
    private readonly Lock _sync = new();

    // Per-file parsed entries, keyed the same way the file provider is
    // (culture name, or "default"). Populated lazily on first lookup and
    // refreshed in full, one key at a time, by OnFileChanged — this is
    // what lets a hot-reload of one culture's file leave every other
    // cached file (and, on failure, that file's own last known-good
    // content) untouched.
    private readonly Dictionary<string, IReadOnlyDictionary<string, string>> _fileCache = new(StringComparer.OrdinalIgnoreCase);

    private CultureInfo _currentCulture;
    private IReadOnlyDictionary<string, string>? _translations;
    private bool _disposed;

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public CultureInfo CurrentCulture {
        get {
            lock (_sync) {
                return _currentCulture;
            }
        }
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

        _fileProvider.Changed += OnFileChanged;
    }

    /// <inheritdoc />
    public void Dispose() {
        if (_disposed) {
            return;
        }

        _disposed = true;

        _fileProvider.Changed -= OnFileChanged;
    }

    /// <inheritdoc />
    public void SetCulture(CultureInfo culture) {
        lock (_sync) {
            if (Equals(_currentCulture, culture)) {
                return;
            }

            // Invalidates the cached dictionary — the next lookup resolves
            // the fallback chain again, now against the new culture.
            _translations = null;
            _currentCulture = culture;
        }

        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        // Raised outside the lock — subscriber code (WPF bindings) must
        // not run while _sync is held.
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentCulture)));

        // Invalidates ALL indexer bindings — forces every {Binding [Key]} to re-evaluate
        PropertyChanged?.Invoke(this, IndexerArgs);
    }

    /// <inheritdoc />
    public void SetCulture(string cultureName) {
        SetCulture(CultureInfo.GetCultureInfo(cultureName));
    }

    private IReadOnlyDictionary<string, string> GetTranslations() {
        lock (_sync) {
            return _translations ??= LoadTranslations();
        }
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
            var entries = GetFileEntries(key);
            if (entries is null) {
                continue;
            }

            foreach (var (translationKey, translationValue) in entries) {
                merged[translationKey] = translationValue;
            }
        }

        return merged;
    }

    /// <summary>
    ///     Returns the parsed entries for <paramref name="key"/>'s file,
    ///     from the per-file cache when already loaded, otherwise reading
    ///     and parsing it through <see cref="_fileProvider"/> and caching
    ///     the result on success.
    /// </summary>
    private IReadOnlyDictionary<string, string>? GetFileEntries(string key) {
        if (_fileCache.TryGetValue(key, out var cached)) {
            return cached;
        }

        if (!_fileProvider.TryRead(key, out var json)) {
            return null;
        }

        var parsed = TryParseFile(key, json);
        if (parsed is not null) {
            _fileCache[key] = parsed;
        }

        return parsed;
    }

    /// <summary>
    ///     Parses <paramref name="json"/> — a JSON object grouping entries
    ///     by section (<c>{ "SectionName": [ { "Key1": "Text1" } ] }</c>)
    ///     — into a flat lookup dictionary, logging the reason (exception,
    ///     when raised, a shape mismatch, or a key duplicated within the
    ///     same section) and returning <see langword="null"/> when parsing
    ///     fails.
    /// </summary>
    /// <remarks>
    ///     A key repeated across two <em>different</em> sections is not an
    ///     error — the section grouping is a purely organizational,
    ///     on-disk concern, invisible to lookups — but whichever section
    ///     is enumerated last silently wins for that flat key, so
    ///     translation authors must still keep flat keys unique across the
    ///     whole file.
    /// </remarks>
    private IReadOnlyDictionary<string, string>? TryParseFile(string key, string json) {
        Dictionary<string, List<Dictionary<string, string>>>? sections;
        try {
            sections = JsonSerializer.Deserialize<Dictionary<string, List<Dictionary<string, string>>>>(json, ParseOptions);
        }
        catch (JsonException exception) {
            CommonLog.JsonDeserializationFailure(_logger, typeof(Dictionary<string, List<Dictionary<string, string>>>), exception, tag: key);

            return null;
        }

        if (sections is null) {
            CommonLog.Warning(_logger, message: "Translation file deserialized to null — expected a JSON object grouping keys by section.", tag: key);

            return null;
        }

        var flattened = new Dictionary<string, string>();
        foreach (var (sectionName, entries) in sections) {
            // Duplicate detection is scoped to this section only — a key
            // repeated across two different sections is allowed (see
            // remarks above), so the set is recreated for every section.
            var seenInSection = new HashSet<string>(StringComparer.Ordinal);

            if (entries is null) {
                CommonLog.Warning(_logger, message: $"Section '{sectionName}' deserialized to null — expected an array of entry objects.", tag: key);

                return null;
            }

            for (var index = 0; index < entries.Count; index++) {
                var entry = entries[index];
                if (entry is null) {
                    CommonLog.Warning(_logger, message: $"Entry at index {index} in section '{sectionName}' deserialized to null — expected an object mapping keys to text.", tag: key);

                    return null;
                }

                foreach (var (entryKey, entryValue) in entry) {
                    if (!seenInSection.Add(entryKey)) {
                        CommonLog.Warning(_logger, message: $"Duplicate key '{entryKey}' in section '{sectionName}'.", tag: key);

                        return null;
                    }

                    flattened[entryKey] = entryValue;
                }
            }
        }

        return flattened;
    }

    /// <summary>
    ///     Handles <see cref="ITranslationFileProvider.Changed"/>:
    ///     reloads the changed file in full and, on success, replaces its
    ///     cached entries and raises the "Item[]" refresh signal. A file
    ///     outside the current culture's fallback chain is simply evicted
    ///     from the cache (so a later <see cref="SetCulture(CultureInfo)"/>
    ///     picks up the fresh content) without triggering a reload or a
    ///     refresh signal, since nothing currently displayed depends on it.
    /// </summary>
    private void OnFileChanged(object? sender, TranslationFileChangedEventArgs e) {
        lock (_sync) {
            if (!IsInCurrentFallbackChainLocked(e.Key)) {
                _fileCache.Remove(e.Key);

                return;
            }
        }

        // File I/O and parsing happen off the lock — they do not touch
        // shared state, and holding _sync across disk reads would block
        // every other culture's lookup/reload for no benefit.
        if (!_fileProvider.TryRead(e.Key, out var json)) {
            CommonLog.Warning(_logger, message: "Translation file changed but could not be read; keeping the last known-good state.", tag: e.Key);

            return;
        }

        var parsed = TryParseFile(e.Key, json);
        if (parsed is null) {
            // TryParseFile already logged the reason. Keep the last
            // known-good cached entries and do not fire the refresh
            // signal — the UI keeps showing exactly what it did before.
            return;
        }

        lock (_sync) {
            _fileCache[e.Key] = parsed;
            _translations = null; // forces the next lookup to re-merge the fallback chain with the fresh entries
        }

        // Raised outside the lock — subscriber code (WPF bindings) must
        // not run while _sync is held.
        PropertyChanged?.Invoke(this, IndexerArgs);
    }

    /// <summary>
    ///     Caller must hold <see cref="_sync"/>.
    /// </summary>
    private bool IsInCurrentFallbackChainLocked(string key) {
        return GetCultureFallbackChain(_currentCulture).Contains(key, StringComparer.OrdinalIgnoreCase);
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

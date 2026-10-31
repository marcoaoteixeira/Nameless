using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.Configuration;
using Nameless.Contracts;

namespace Nameless.Windows.Localization;

/// <summary>
///     <see cref="ITranslator"/> implementation backed by a .NET
///     <see cref="System.Resources.ResourceManager"/>. Supports runtime
///     culture switching: calling <see cref="SetCulture(CultureInfo)"/>
///     raises <see cref="INotifyPropertyChanged.PropertyChanged"/> with
///     <c>"Item[]"</c> so all WPF indexer bindings re-evaluate
///     automatically.
/// </summary>
/// <remarks>
///     Use <see cref="TranslatorExtension"/> in XAML to bind to this localizer.
/// </remarks>
public sealed class ResourceTranslator : ITranslator {
    // Cached event args — avoids allocating on every culture change
    private static readonly PropertyChangedEventArgs IndexerArgs = new("Item[]");
    
    private readonly ILogger<ResourceTranslator> _logger;
    private readonly TranslatorOptions _options;
    
    private readonly Lazy<ResourceManager> _resourceManager;
    private CultureInfo _currentCulture;

    private ResourceManager ResourceManager => _resourceManager.Value;

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

            // Retrieves the resource value or fallback makes missing
            // keys visible during development
            var resource = ResourceManager.GetString(key, _currentCulture);
            if (!string.IsNullOrWhiteSpace(resource)) {
                return string.Format(resource, parameters);
            }

            CommonLog.Debug(_logger, message: $"Missing key '{key}'", GetType().Tag);

            return $"[{key}]";
        }
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="ResourceTranslator"/>
    ///     class.
    /// </summary>
    /// <param name="options">
    ///     The options.
    /// </param>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    public ResourceTranslator(IOptions<TranslatorOptions> options, ILogger<ResourceTranslator> logger) {
        _options = options.Value;
        _logger = logger;
        _currentCulture = CultureInfo.CurrentUICulture;
        _resourceManager = new Lazy<ResourceManager>(CreateResourceManager);
    }

    /// <inheritdoc />
    public void SetCulture(CultureInfo culture) {
        if (Equals(_currentCulture, culture)) {
            return;
        }

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

    private ResourceManager CreateResourceManager() {
        if (string.IsNullOrWhiteSpace(_options.ResourceFullName)) {
            throw new MissingConfigurationException(
                section: ConfigurationSectionNameAttribute.GetSectionName<TranslatorOptions>(),
                key: nameof(TranslatorOptions.ResourceFullName)
            );
        }

        var resourceType = Type.GetType(_options.ResourceFullName)
                           ?? throw new InvalidOperationException($"Couldn't locate resource '{_options.ResourceFullName}'.");

        return new ResourceManager(
            Throws.When.NullOrWhiteSpace(resourceType.FullName),
            resourceType.Assembly
        );
    }
}
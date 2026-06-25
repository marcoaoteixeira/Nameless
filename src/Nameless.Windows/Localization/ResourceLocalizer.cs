using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Options;
using Nameless.Attributes;
using Nameless.Configuration;

namespace Nameless.Windows.Localization;

/// <summary>
///     Singleton localization manager that supports runtime culture switching.
///     Bind to it in XAML via: {Binding [KeyName], Source={x:Static loc:L10N.Instance}}
/// </summary>
public sealed class ResourceLocalizer : ILocalizer {
    // Cached event args — avoids allocating on every culture change
    private static readonly PropertyChangedEventArgs IndexerArgs = new("Item[]");
    private readonly ResourceLocalizerOptions _options;
    private readonly Lazy<ResourceManager> _resourceManager;
    private CultureInfo _currentCulture;

    private ResourceManager ResourceManager => _resourceManager.Value;

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// The currently active culture.
    /// </summary>
    public CultureInfo CurrentCulture {
        get => _currentCulture;
        private set => SetField(ref _currentCulture, value);
    }

    public string this[string key] => this[key, parameters: []];

    /// <summary>
    ///     Indexer — the main binding target.
    ///     Usage in XAML: {Binding [MyKey], Source={x:Static loc:L10N.Instance}}
    /// </summary>
    public string this[string key, params object[] parameters] {
        get {
            if (string.IsNullOrWhiteSpace(key)) {
                return string.Empty;
            }

            // Retrieves the resource value or fallback makes missing
            // keys visible during development
            var resource = ResourceManager.GetString(key, _currentCulture);
            if (string.IsNullOrWhiteSpace(resource)) { return $"[{key}]"; }

            return parameters.Length > 0
                ? string.Format(resource, parameters)
                : resource;
        }
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="ResourceLocalizer"/>
    ///     class.
    /// </summary>
    /// <param name="options">
    ///     The options.
    /// </param>
    public ResourceLocalizer(IOptions<ResourceLocalizerOptions> options) {
        _options = options.Value;
        _currentCulture = CultureInfo.CurrentUICulture;
        _resourceManager = new Lazy<ResourceManager>(CreateResourceManager);
    }

    /// <summary>
    /// Switches the application culture at runtime and notifies all bindings.
    /// </summary>
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

    /// <summary>
    ///     Convenience overload accepting a culture string, e.g. "pt-PT".
    /// </summary>
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
                section: ConfigurationSectionNameAttribute.GetSectionName<ResourceLocalizerOptions>(),
                key: nameof(ResourceLocalizerOptions.ResourceFullName)
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
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
using Nameless.WinApp.Resources;

namespace Nameless.WinApp;

/// <summary>
///     Singleton localization manager that supports runtime culture switching.
///     Bind to it in XAML via: {Binding [KeyName], Source={x:Static loc:L10N.Instance}}
/// </summary>
public sealed class L10N : INotifyPropertyChanged {
    private static readonly Lazy<L10N> Current = new(() => new L10N());
    // Cached event args — avoids allocating on every culture change
    private static readonly PropertyChangedEventArgs IndexerArgs = new("Item[]");

    public static L10N Instance => Current.Value;

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly ResourceManager _resourceManager;
    private CultureInfo _currentCulture;

    /// <summary>
    /// The currently active culture.
    /// </summary>
    public CultureInfo CurrentCulture {
        get => _currentCulture;
        private set => SetField(ref _currentCulture, value);
    }

    /// <summary>
    ///     Indexer — the main binding target.
    ///     Usage in XAML: {Binding [MyKey], Source={x:Static loc:L10N.Instance}}
    /// </summary>
    public string this[string key] {
        get {
            if (string.IsNullOrWhiteSpace(key)) {
                return string.Empty;
            }

            return _resourceManager.GetString(key, _currentCulture)
                   ?? $"[{key}]"; // fallback makes missing keys visible during development
        }
    }

    private L10N() {
        // Reuse the ResourceManager from the auto-generated .resx class.
        // Replace with: new ResourceManager("YourApp.Properties.Resources", Assembly.GetExecutingAssembly())
        // if you prefer not to depend on the generated class.
        _resourceManager = Translations.ResourceManager;
        _currentCulture = CultureInfo.CurrentUICulture;
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
}

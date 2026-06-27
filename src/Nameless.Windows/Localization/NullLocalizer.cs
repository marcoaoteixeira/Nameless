using System.ComponentModel;
using System.Globalization;
using Nameless.Registration;

namespace Nameless.Windows.Localization;

/// <summary>
///     Null-object implementation of <see cref="ILocalizer"/>.
///     Returns the resource key as-is (applying any format arguments) and
///     ignores culture-switch requests. Used as the default before a real
///     localizer is registered.
/// </summary>
[IgnoreAssemblyScan]
public sealed class NullLocalizer : ILocalizer {
    /// <summary>
    ///     Gets the singleton instance.
    /// </summary>
    public static ILocalizer Instance { get; } = new NullLocalizer();

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged { add { } remove { } }

    /// <inheritdoc />
    public string this[string key] => key;

    /// <inheritdoc />
    public string this[string key, params object[] parameters] => parameters.Length > 0
        ? string.Format(key, parameters)
        : key;

    /// <inheritdoc />
    public CultureInfo CurrentCulture => CultureInfo.CurrentUICulture;

    static NullLocalizer() { }

    private NullLocalizer() { }

    /// <inheritdoc />
    public void SetCulture(CultureInfo culture) { }

    /// <inheritdoc />
    public void SetCulture(string cultureName) { }
}
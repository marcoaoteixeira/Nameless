using System.ComponentModel;
using System.Globalization;
using Nameless.Registration;

namespace Nameless.Windows.Localization;

/// <summary>
///     Null-object implementation of <see cref="ITranslator"/>.
///     Returns the resource key as-is (applying any format arguments) and
///     ignores culture-switch requests. Used as the default before a real
///     localizer is registered.
/// </summary>
[SkipAssemblyScan]
public sealed class NullTranslator : ITranslator {
    /// <summary>
    ///     Gets the singleton instance.
    /// </summary>
    public static ITranslator Instance { get; } = new NullTranslator();

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

    static NullTranslator() { }

    private NullTranslator() { }

    /// <inheritdoc />
    public void SetCulture(CultureInfo culture) { }

    /// <inheritdoc />
    public void SetCulture(string cultureName) { }
}
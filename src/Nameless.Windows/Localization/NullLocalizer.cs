using System.ComponentModel;
using System.Globalization;
using Nameless.Registration;

namespace Nameless.Windows.Localization;

[IgnoreAssemblyScan]
public sealed class NullLocalizer : ILocalizer {
    public static ILocalizer Instance { get; } = new NullLocalizer();

    public event PropertyChangedEventHandler? PropertyChanged { add { } remove { } }

    public string this[string key] => key;

    public string this[string key, params object[] parameters] => parameters.Length > 0
        ? string.Format(key, parameters)
        : key;

    public CultureInfo CurrentCulture => CultureInfo.CurrentUICulture;

    static NullLocalizer() { }

    private NullLocalizer() { }

    public void SetCulture(CultureInfo culture) { }

    public void SetCulture(string cultureName) { }
}
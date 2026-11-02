using System.ComponentModel;
using System.Globalization;
using Nameless.Windows.Localization;

namespace Nameless.WinApp.Tests.Tooling;

// Real ITranslator implementations pull from resx files; most handlers only
// care that a key resolves to *some* string, so a trivial echo is enough
// and keeps tests from depending on resource content.
internal sealed class FakeTranslator : ITranslator {
    public event PropertyChangedEventHandler? PropertyChanged {
        add { }
        remove { }
    }

    public string this[string key] => key;

    public string this[string key, params object[] parameters] => string.Format(CultureInfo.InvariantCulture, key, parameters);

    public CultureInfo CurrentCulture => CultureInfo.InvariantCulture;

    public void SetCulture(CultureInfo culture) { }

    public void SetCulture(string cultureName) { }
}

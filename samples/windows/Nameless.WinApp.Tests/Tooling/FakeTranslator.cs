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

    // Goes through the real TranslatorFactory/Translator<T>/SectionTranslator
    // pipeline (ADR-003), so the composed key a test sees back is the exact
    // same string the old full-key ITranslator calls used to produce —
    // existing assertions built against FakeTranslator's echo keep passing
    // unchanged, which doubles as a regression check on the #199 migration.
    public static ITranslator<T> For<T>() {
        return new TranslatorFactory(new FakeTranslator()).Create<T>();
    }
}

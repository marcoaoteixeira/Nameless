using System.ComponentModel;
using System.Globalization;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Windows.Localization;

/// <summary>
///     Pins <see cref="TranslatorFeatureOptions.Translator"/>'s assembly-scan
///     path excluding <see cref="ITranslator{T}"/> implementations the same
///     way <see cref="TranslatorFeatureOptions.WithTranslator(Type)"/>
///     rejects them explicitly (ADR-003) — a scanned type implementing
///     <see cref="ITranslator{T}"/> must never become the root translator.
/// </summary>
[UnitTest]
public class TranslatorFeatureOptionsTests {
    // Stands in for a legitimate, scannable root ITranslator.
    public sealed class ScanFixtureRootTranslator : ITranslator {
        public event PropertyChangedEventHandler? PropertyChanged { add { } remove { } }

        public string this[string key] => key;

        public string this[string key, params object[] parameters] => key;

        public CultureInfo CurrentCulture => CultureInfo.InvariantCulture;

        public void SetCulture(CultureInfo culture) { }

        public void SetCulture(string cultureName) { }
    }

    // Stands in for a type that implements ITranslator<T> and gets picked
    // up by the scan for ITranslator (ITranslator<T> extends ITranslator)
    // alongside the legitimate root above.
    public sealed class ScanFixtureTypedTranslator : ITranslator<TranslatorFeatureOptionsTests> {
        public event PropertyChangedEventHandler? PropertyChanged { add { } remove { } }

        public string this[string key] => key;

        public string this[string key, params object[] parameters] => key;

        public CultureInfo CurrentCulture => CultureInfo.InvariantCulture;

        public void SetCulture(CultureInfo culture) { }

        public void SetCulture(string cultureName) { }
    }

    [Fact]
    public void Translator_WithAssemblyScan_ExcludesITranslatorOfTImplementations() {
        // arrange: both fixtures above are scannable concrete ITranslator
        // implementations; without the fix, SingleOrDefault() would either
        // pick ScanFixtureTypedTranslator (building a root -> factory ->
        // root cycle, ADR-003) or throw because more than one candidate
        // matches.
        var sut = new TranslatorFeatureOptions().WithAssemblyFrom<TranslatorFeatureOptionsTests>();

        // act
        var translator = sut.Translator;

        // assert
        Assert.Equal(typeof(ScanFixtureRootTranslator), translator);
    }
}

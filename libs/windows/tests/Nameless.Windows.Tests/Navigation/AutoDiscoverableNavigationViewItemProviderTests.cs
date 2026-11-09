using System.ComponentModel;
using System.Globalization;
using Nameless.Testing.Tools.Attributes;
using Nameless.Windows.Localization;
using Wpf.Ui.Controls;

namespace Nameless.Windows.Navigation;

// Entry.Build() constructs a real Wpf.Ui.Controls.NavigationViewItem, which
// requires an STA thread (WPF control instantiation). Exercising the
// resolution logic directly through Entry.ResolveTitle/ResolveToolTip keeps
// these tests free of that UI-thread requirement while still covering the
// exact code path TranslatorHostFeature's NullTranslator/JsonTranslator
// wiring drives at runtime.
[UnitTest]
public class AutoDiscoverableNavigationViewItemProviderTests {
    [Fact]
    public void ResolveTitleAndToolTip_WhenNoResourceTypeButResourceNamesSet_AndTranslatorProvided_UsesTranslator() {
        // arrange
        var translator = new MapTranslator(new Dictionary<string, string> {
            ["Fixture_Title"] = "Translated Title",
            ["Fixture_ToolTip"] = "Translated ToolTip"
        });

        var entry = CreateFixtureEntry();

        // act & assert
        Assert.Equal("Translated Title", entry.ResolveTitle(translator));
        Assert.Equal("Translated ToolTip", entry.ResolveToolTip(translator));
    }

    [Fact]
    public void ResolveTitleAndToolTip_WhenNoResourceTypeButResourceNamesSet_AndNoTranslatorProvided_FallsBackToLiteralResourceName() {
        // arrange: no translator — matches an unconfigured host, where
        // NavigationHostFeature wires a null ITranslator into the provider.
        var entry = CreateFixtureEntry();

        // act & assert
        Assert.Equal("Fixture_Title", entry.ResolveTitle(translator: null));
        Assert.Equal("Fixture_ToolTip", entry.ResolveToolTip(translator: null));
    }

    [Fact]
    public void ResolveTitleAndToolTip_WhenResourceTypeIsSet_IgnoresTranslator_UsesAttributeResolution() {
        // arrange: ResourceType present — the resx-only path still wins,
        // even when a translator is available, matching the attribute's
        // own precedence.
        var translator = new MapTranslator(new Dictionary<string, string> {
            ["NavigationViewItemAttribute_Title"] = "Should Not Be Used"
        });

        var attribute = new NavigationViewItemAttribute {
            Icon = SymbolRegular.Settings48,
            TitleResourceName = "NavigationViewItemAttribute_Title",
            ResourceType = typeof(global::Nameless.Windows.Windows.Navigation.Resources.Resources)
        };
        var entry = new AutoDiscoverableNavigationViewItemProvider.Entry(typeof(AutoDiscoverableNavigationViewItemProviderTests), attribute);

        // act & assert
        Assert.Equal("Title Okey-Dokey", entry.ResolveTitle(translator));
    }

    private static AutoDiscoverableNavigationViewItemProvider.Entry CreateFixtureEntry() {
        var attribute = new NavigationViewItemAttribute {
            Icon = SymbolRegular.Settings48,
            Placement = Placement.Footer,
            TitleResourceName = "Fixture_Title",
            ToolTipResourceName = "Fixture_ToolTip"
        };

        return new AutoDiscoverableNavigationViewItemProvider.Entry(typeof(AutoDiscoverableNavigationViewItemProviderTests), attribute);
    }

    // Trivial key -> value lookup, just enough to prove the provider
    // actually calls into ITranslator instead of falling back to the
    // attribute's own resx-only resolution.
    private sealed class MapTranslator(Dictionary<string, string> values) : ITranslator {
        public event PropertyChangedEventHandler? PropertyChanged { add { } remove { } }

        public string this[string key] => values.GetValueOrDefault(key, key);

        public string this[string key, params object[] parameters] => this[key];

        public CultureInfo CurrentCulture => CultureInfo.InvariantCulture;

        public void SetCulture(CultureInfo culture) { }

        public void SetCulture(string cultureName) { }
    }
}

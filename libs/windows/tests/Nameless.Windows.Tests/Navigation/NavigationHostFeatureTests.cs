using System.ComponentModel;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Hosting;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Mockers.Hosting;
using Nameless.Windows.Hosting.Features.Navigation;
using Nameless.Windows.Localization;
using Wpf.Ui;
using Wpf.Ui.Abstractions;
using Wpf.Ui.Controls;

namespace Nameless.Windows.Navigation;

// Pins the DI wiring behind #190's nav-label fix: if the factory ever
// reverted to "new AutoDiscoverableNavigationViewItemProvider(assemblies)"
// with no translator, pt-BR labels would break again while every other
// existing test kept passing.
[UnitTest]
public class NavigationHostFeatureTests {
    [Fact]
    public void Register_ResolvesNavigationViewItemProvider_WiredWithTheContainerTranslator() {
        // arrange
        var translator = new StubTranslator();

        var builder = new HostApplicationBuilderMocker()
            .WithServices(services => {
                services.AddLogging();
                services.AddSingleton<ITranslator>(translator);
            })
            .Build();

        var feature = new NavigationHostFeature();

        feature.Configure(options => options.WithUseAssemblyScan(false)
                                            .WithNavigationWindow<StubNavigationWindow>());

        // act
        feature.Register(builder, new HostFeatureContext([], NullLogger.Instance));

        var provider = builder.Services.BuildServiceProvider();
        var sut = provider.GetRequiredService<INavigationViewItemProvider>();

        // assert
        var autoDiscoverable = Assert.IsType<AutoDiscoverableNavigationViewItemProvider>(sut);
        Assert.Same(translator, autoDiscoverable.Translator);
    }

    private sealed class StubNavigationWindow : INavigationWindow {
        public void CloseWindow() { }

        public INavigationView GetNavigation() => throw new NotSupportedException();

        public bool Navigate(Type pageType) => throw new NotSupportedException();

        public void SetPageService(INavigationViewPageProvider navigationViewPageProvider) { }

        public void SetServiceProvider(IServiceProvider serviceProvider) { }

        public void ShowWindow() { }
    }

    private sealed class StubTranslator : ITranslator {
        public event PropertyChangedEventHandler? PropertyChanged { add { } remove { } }

        public string this[string key] => key;

        public string this[string key, params object[] parameters] => key;

        public CultureInfo CurrentCulture => CultureInfo.InvariantCulture;

        public void SetCulture(CultureInfo culture) { }

        public void SetCulture(string cultureName) { }
    }
}

using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Nameless.Windows.Localization;
using Nameless.Windows.Localization.Json;

namespace Nameless.WinApp.Resources.loc;

/// <summary>
///     Pins <see cref="JsonTranslator"/> against the sample app's real,
///     on-disk translation files through <see cref="FileSystemTranslationFileProvider"/>
///     (as opposed to <c>TranslatorHostFeatureTests</c>, which only proves
///     the DI wiring against synthetic content) — a known nav-item key
///     must resolve correctly for both the default and pt-BR cultures.
/// </summary>
[IntegrationTest]
public class FileSystemTranslationFileProviderSampleAppTests {
    [Fact]
    public void Translator_WithRealSampleAppFiles_ResolvesKnownKey_ForDefaultAndPtBrCultures() {
        // arrange
        var options = Options.Create(new TranslatorOptions { RootPath = "Resources\\loc" });
        var fileProvider = new FileSystemTranslationFileProvider(options, NullLogger<FileSystemTranslationFileProvider>.Instance);
        var sut = new JsonTranslator(fileProvider, NullLogger<JsonTranslator>.Instance);

        // act & assert
        sut.SetCulture("en-US");
        Assert.Equal("Settings", sut["AppConfigurationPage_NavigationViewItem_Title"]);

        sut.SetCulture("pt-BR");
        Assert.Equal("Configurações", sut["AppConfigurationPage_NavigationViewItem_Title"]);
    }
}

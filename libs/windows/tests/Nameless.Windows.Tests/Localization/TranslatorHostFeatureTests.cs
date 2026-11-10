using System.ComponentModel;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Hosting;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Mockers.Hosting;
using SysDirectory = System.IO.Directory;
using SysFile = System.IO.File;
using SysPath = System.IO.Path;

namespace Nameless.Windows.Localization;

/// <summary>
///     Pins <see cref="TranslatorHostFeature"/>'s DI wiring now that it
///     resolves <see cref="JsonTranslator"/> instead of the retired
///     <c>ResourceTranslator</c> (#190's acceptance criteria).
/// </summary>
[IntegrationTest]
public sealed class TranslatorHostFeatureTests : IDisposable {
    private readonly string _rootPath;

    public TranslatorHostFeatureTests() {
        _rootPath = SysPath.Combine(SysPath.GetTempPath(), $"nameless-translator-host-feature-tests-{Guid.NewGuid():N}");

        SysDirectory.CreateDirectory(_rootPath);
    }

    public void Dispose() {
        SysDirectory.Delete(_rootPath, recursive: true);
    }

    [Fact]
    public void Register_WithValidRootPath_ResolvesJsonTranslator_ReadingRealFilesForBothCultures() {
        // arrange
        SysFile.WriteAllText(SysPath.Combine(_rootPath, "default.json"), """{ "Key1": "Text1" }""");
        SysFile.WriteAllText(SysPath.Combine(_rootPath, "pt-BR.json"), """{ "Key1": "TextoPT-BR" }""");

        var configuration = new ConfigurationManager();
        configuration.AddInMemoryCollection(new Dictionary<string, string?> {
            ["Translator:RootPath"] = _rootPath
        });

        var builder = new HostApplicationBuilderMocker()
            .WithServices(services => services.AddLogging())
            .WithConfiguration(configuration)
            .Build();

        var feature = new TranslatorHostFeature();

        // act: assembly scanning only inspects the given assemblies' own
        // exported types (not referenced assemblies), so Nameless.Windows
        // (where JsonTranslator lives) must be passed explicitly — the
        // same way the real host passes its app + referenced assemblies.
        feature.Register(builder, new HostFeatureContext([typeof(JsonTranslator).Assembly], NullLogger.Instance));

        var provider = builder.Services.BuildServiceProvider();
        var sut = provider.GetRequiredService<ITranslator>();

        sut.SetCulture("pt-BR");

        // assert
        Assert.IsType<JsonTranslator>(sut);
        Assert.Equal("TextoPT-BR", sut["Key1"]);

        sut.SetCulture("en-US");

        Assert.Equal("Text1", sut["Key1"]);
    }

    [Fact]
    public void Register_WithNoTranslatorConfiguration_ResolvesNullTranslator_EvenWhenJsonTranslatorIsScannable() {
        // arrange: an empty configuration — matches an unconfigured host.
        // The scanned assembly list mirrors what WinHostFactory-built real
        // hosts actually pass (Nameless.Windows, where JsonTranslator
        // lives), so this proves the fallback doesn't depend on
        // JsonTranslator being absent from the scan — it must be skipped
        // purely because the "Translator" configuration section is missing.
        var configuration = new ConfigurationManager();
        var builder = new HostApplicationBuilderMocker()
            .WithServices(services => services.AddLogging())
            .WithConfiguration(configuration)
            .Build();

        var feature = new TranslatorHostFeature();

        // act
        feature.Register(builder, new HostFeatureContext([typeof(JsonTranslator).Assembly], NullLogger.Instance));

        var provider = builder.Services.BuildServiceProvider();
        var sut = provider.GetRequiredService<ITranslator>();

        // assert
        Assert.Same(NullTranslator.Instance, sut);
    }

    [Fact]
    public void Register_WithExplicitCustomTranslatorAndNoConfiguration_ResolvesCustomTranslator() {
        // arrange: no "Translator" configuration section — matches a
        // consumer whose custom ITranslator (e.g. DB-backed) needs no such
        // section at all. Assembly scanning is off, so WithTranslator<T>()
        // is the only source for the type.
        var configuration = new ConfigurationManager();
        var builder = new HostApplicationBuilderMocker()
            .WithServices(services => services.AddLogging())
            .WithConfiguration(configuration)
            .Build();

        var feature = new TranslatorHostFeature();

        feature.Configure(options => options.WithUseAssemblyScan(false)
                                             .WithTranslator<CustomTranslator>());

        // act
        feature.Register(builder, new HostFeatureContext([typeof(JsonTranslator).Assembly], NullLogger.Instance));

        var provider = builder.Services.BuildServiceProvider();
        var sut = provider.GetRequiredService<ITranslator>();

        // assert
        Assert.IsType<CustomTranslator>(sut);
    }

    // Stands in for a consumer-owned ITranslator (e.g. DB-backed) that
    // doesn't depend on the "Translator" configuration section at all.
    private sealed class CustomTranslator : ITranslator {
        public event PropertyChangedEventHandler? PropertyChanged { add { } remove { } }

        public string this[string key] => key;

        public string this[string key, params object[] parameters] => key;

        public CultureInfo CurrentCulture => CultureInfo.InvariantCulture;

        public void SetCulture(CultureInfo culture) { }

        public void SetCulture(string cultureName) { }
    }
}

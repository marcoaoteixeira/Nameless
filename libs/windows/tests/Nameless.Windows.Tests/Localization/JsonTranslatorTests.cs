using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Testing.Tools.Attributes;
using Nameless.Windows.Localization;
using Nameless.Windows.Localization.Json;

namespace Nameless.Windows.Tests.Localization;

/// <summary>
///     Pins <see cref="JsonTranslator"/>'s lookup, culture fallback,
///     missing-key and format behavior against a fake, in-memory
///     <see cref="ITranslationFileProvider"/> — zero behavioral drift from
///     <c>ResourceTranslator</c> is the point of this story.
/// </summary>
[UnitTest]
public class JsonTranslatorTests {
    /// <summary>
    ///     In-memory stand-in for a real (future, hot-reload capable)
    ///     file provider. Keyed exactly like the production seam will be:
    ///     by culture name, or the literal "default".
    /// </summary>
    private sealed class FakeTranslationFileProvider : ITranslationFileProvider {
        private readonly Dictionary<string, string> _files = new(StringComparer.OrdinalIgnoreCase);

        public event EventHandler<TranslationFileChangedEventArgs>? Changed;

        public FakeTranslationFileProvider With(string key, string json) {
            _files[key] = json;

            return this;
        }

        public bool TryRead(string key, [NotNullWhen(true)] out string? content) {
            return _files.TryGetValue(key, out content);
        }

        /// <summary>
        ///     Simulates a hot-reload-capable provider noticing a change —
        ///     raises <see cref="Changed"/> for <paramref name="key"/>.
        /// </summary>
        public void Raise(string key) {
            Changed?.Invoke(this, new TranslationFileChangedEventArgs(key));
        }
    }

    private static JsonTranslator CreateSut(ITranslationFileProvider fileProvider) {
        return new JsonTranslator(fileProvider, NullLogger<JsonTranslator>.Instance);
    }

    [Fact]
    public void Indexer_WhenKeyExistsInDefaultFile_ReturnsItsValue() {
        // arrange
        var fileProvider = new FakeTranslationFileProvider()
            .With("default", """{ "Key1": "Text1" }""");

        var sut = CreateSut(fileProvider);

        // act
        var result = sut["Key1"];

        // assert
        Assert.Equal("Text1", result);
    }

    [Fact]
    public void Indexer_WhenKeyExistsInSpecificCultureFile_PrefersItOverNeutralAndDefault() {
        // arrange: all three files define "Key1" with a different value —
        // only the most specific one (pt-BR) must win.
        var fileProvider = new FakeTranslationFileProvider()
            .With("pt-BR", """{ "Key1": "TextoPT-BR" }""")
            .With("pt", """{ "Key1": "TextoPT" }""")
            .With("default", """{ "Key1": "Text1" }""");

        var sut = CreateSut(fileProvider);
        sut.SetCulture("pt-BR");

        // act
        var result = sut["Key1"];

        // assert
        Assert.Equal("TextoPT-BR", result);
    }

    [Fact]
    public void Indexer_WhenKeyMissingFromSpecificCultureFile_FallsBackPerKeyToNeutralOrDefaultFile() {
        // arrange: pt-BR exists but only defines "Key1" — "Key2" must
        // still resolve from "pt", and "Key3" from "default", instead of
        // the whole pt-BR file blocking fallback for keys it lacks.
        var fileProvider = new FakeTranslationFileProvider()
            .With("pt-BR", """{ "Key1": "TextoPT-BR" }""")
            .With("pt", """{ "Key1": "TextoPT", "Key2": "TextoPT2" }""")
            .With("default", """{ "Key1": "Text1", "Key2": "Text2", "Key3": "Text3" }""");

        var sut = CreateSut(fileProvider);
        sut.SetCulture("pt-BR");

        // act & assert
        Assert.Equal("TextoPT-BR", sut["Key1"]);
        Assert.Equal("TextoPT2", sut["Key2"]);
        Assert.Equal("Text3", sut["Key3"]);
    }

    [Fact]
    public void Indexer_WhenSpecificCultureFileIsMalformedJson_LogsAndFallsBackToNeutralOrDefaultFile() {
        // arrange: pt-BR is malformed (wrong shape — array, not object) —
        // the lookup must not throw and must still resolve "Key1" from
        // "pt" instead of crashing on every binding re-read.
        var fileProvider = new FakeTranslationFileProvider()
            .With("pt-BR", """[ "not", "an", "object" ]""")
            .With("pt", """{ "Key1": "TextoPT" }""")
            .With("default", """{ "Key1": "Text1" }""");

        var sut = CreateSut(fileProvider);
        sut.SetCulture("pt-BR");

        // act
        var result = sut["Key1"];

        // assert
        Assert.Equal("TextoPT", result);
    }

    [Fact]
    public void Indexer_WhenSpecificCultureFileMissing_FallsBackToNeutralCultureFile() {
        // arrange: no "pt-BR" file, but "pt" and "default" exist.
        var fileProvider = new FakeTranslationFileProvider()
            .With("pt", """{ "Key1": "TextoPT" }""")
            .With("default", """{ "Key1": "Text1" }""");

        var sut = CreateSut(fileProvider);
        sut.SetCulture("pt-BR");

        // act
        var result = sut["Key1"];

        // assert: value came from "pt", not "default".
        Assert.Equal("TextoPT", result);
    }

    [Fact]
    public void Indexer_WhenNoCultureFilesExist_FallsBackToDefaultFile() {
        // arrange: neither "pt-BR" nor "pt" exist.
        var fileProvider = new FakeTranslationFileProvider()
            .With("default", """{ "Key1": "Text1" }""");

        var sut = CreateSut(fileProvider);
        sut.SetCulture("pt-BR");

        // act
        var result = sut["Key1"];

        // assert
        Assert.Equal("Text1", result);
    }

    [Fact]
    public void Indexer_WhenKeyAbsentFromResolvedFile_ReturnsBracketedKey() {
        // arrange
        var fileProvider = new FakeTranslationFileProvider()
            .With("default", """{ "Key1": "Text1" }""");

        var sut = CreateSut(fileProvider);

        // act
        var result = sut["Missing"];

        // assert
        Assert.Equal("[Missing]", result);
    }

    [Fact]
    public void Indexer_WithParameters_AppliesStringFormatSubstitution() {
        // arrange
        var fileProvider = new FakeTranslationFileProvider()
            .With("default", """{ "Greeting": "Hello, {0}" }""");

        var sut = CreateSut(fileProvider);

        // act
        var result = sut["Greeting", "Marco"];

        // assert
        Assert.Equal("Hello, Marco", result);
    }

    [Fact]
    public void Indexer_WithParameters_WhenPlaceholderUnsatisfied_ThrowsFormatException() {
        // arrange
        var fileProvider = new FakeTranslationFileProvider()
            .With("default", """{ "Greeting": "Hello, {0}" }""");

        var sut = CreateSut(fileProvider);

        // act & assert: matches ResourceTranslator's current, uncaught behavior.
        Assert.Throws<FormatException>(() => sut["Greeting"]);
    }

    [Fact]
    public void SetCulture_InvalidatesCachedTranslations_SoTheNextLookupResolvesAgainstTheNewCulture() {
        // arrange
        var fileProvider = new FakeTranslationFileProvider()
            .With("pt", """{ "Key1": "TextoPT" }""")
            .With("default", """{ "Key1": "Text1" }""");

        var sut = CreateSut(fileProvider);

        // act: normalize to a culture with no matching file (falls back
        // to "default"), resolving + caching against it first — ensures
        // the environment's own current UI culture cannot make this a
        // no-op.
        sut.SetCulture("en-US");
        _ = sut["Key1"];

        sut.SetCulture("pt-BR");
        var result = sut["Key1"];

        // assert
        Assert.Equal("TextoPT", result);
    }

    [Fact]
    public void SetCulture_RaisesIndexerPropertyChanged_SoBoundControlsRefresh() {
        // arrange
        var fileProvider = new FakeTranslationFileProvider()
            .With("default", """{ "Key1": "Text1" }""");

        var sut = CreateSut(fileProvider);

        // Normalizes the starting culture first — the environment's
        // current UI culture could otherwise already match the culture
        // used in the act step below, making SetCulture a no-op.
        sut.SetCulture("en-US");

        var raised = false;
        sut.PropertyChanged += (_, args) => raised = args.PropertyName == "Item[]";

        // act
        sut.SetCulture("fr-FR");

        // assert
        Assert.True(raised);
    }
}

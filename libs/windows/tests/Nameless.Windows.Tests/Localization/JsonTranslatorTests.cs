using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Logging;
using Nameless.Windows.Localization.Json;

namespace Nameless.Windows.Localization;

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

    private static JsonTranslator CreateSut(ITranslationFileProvider fileProvider, ILogger<JsonTranslator>? logger = null) {
        return new JsonTranslator(fileProvider, logger ?? NullLogger<JsonTranslator>.Instance);
    }

    [Fact]
    public void Indexer_WhenKeyExistsInDefaultFile_ReturnsItsValue() {
        // arrange
        var fileProvider = new FakeTranslationFileProvider()
            .With("default", """{ "Section": { "Key1": "Text1" } }""");

        var sut = CreateSut(fileProvider);

        // act
        var result = sut["Section_Key1"];

        // assert
        Assert.Equal("Text1", result);
    }

    [Fact]
    public void Indexer_WhenKeyExistsInSpecificCultureFile_PrefersItOverNeutralAndDefault() {
        // arrange: all three files define "Key1" with a different value —
        // only the most specific one (pt-BR) must win.
        var fileProvider = new FakeTranslationFileProvider()
            .With("pt-BR", """{ "Section": { "Key1": "TextoPT-BR" } }""")
            .With("pt", """{ "Section": { "Key1": "TextoPT" } }""")
            .With("default", """{ "Section": { "Key1": "Text1" } }""");

        var sut = CreateSut(fileProvider);
        sut.SetCulture("pt-BR");

        // act
        var result = sut["Section_Key1"];

        // assert
        Assert.Equal("TextoPT-BR", result);
    }

    [Fact]
    public void Indexer_WhenKeyMissingFromSpecificCultureFile_FallsBackPerKeyToNeutralOrDefaultFile() {
        // arrange: pt-BR exists but only defines "Key1" — "Key2" must
        // still resolve from "pt", and "Key3" from "default", instead of
        // the whole pt-BR file blocking fallback for keys it lacks.
        var fileProvider = new FakeTranslationFileProvider()
            .With("pt-BR", """{ "Section": { "Key1": "TextoPT-BR" } }""")
            .With("pt", """{ "Section": { "Key1": "TextoPT", "Key2": "TextoPT2" } }""")
            .With("default", """{ "Section": { "Key1": "Text1", "Key2": "Text2", "Key3": "Text3" } }""");

        var sut = CreateSut(fileProvider);
        sut.SetCulture("pt-BR");

        // act & assert
        Assert.Equal("TextoPT-BR", sut["Section_Key1"]);
        Assert.Equal("TextoPT2", sut["Section_Key2"]);
        Assert.Equal("Text3", sut["Section_Key3"]);
    }

    [Fact]
    public void Indexer_WhenSameRelativeKeyExistsInDifferentSections_EachComposesToItsOwnDistinctKey() {
        // arrange: the section name is part of the flat lookup key
        // (TranslationKey.Compose), so the same section-relative key in
        // two different sections composes to two distinct flat keys and
        // never collides — unlike the pre-#199 shape, where the flat key
        // was the section-relative key alone.
        var fileProvider = new FakeTranslationFileProvider()
            .With("default", """
                {
                    "SectionA": { "Key": "FromSectionA" },
                    "SectionB": { "Key": "FromSectionB" }
                }
                """);

        var sut = CreateSut(fileProvider);

        // act & assert
        Assert.Equal("FromSectionA", sut["SectionA_Key"]);
        Assert.Equal("FromSectionB", sut["SectionB_Key"]);
    }

    [Fact]
    public void Indexer_WhenKeyDuplicatedWithinTheSameSection_LogsAndFallsBackToNeutralOrDefaultFile() {
        // arrange: "pt-BR" repeats "Key1" within "SectionA" — default
        // JsonSerializerOptions would silently keep the last value
        // ("Second") instead of failing loudly, so this must be logged and
        // fall back exactly like a malformed file, instead of silently
        // picking one of the values.
        var fileProvider = new FakeTranslationFileProvider()
            .With("pt-BR", """{ "SectionA": { "Key1": "First", "Key1": "Second" } }""")
            .With("pt", """{ "Section": { "Key1": "TextoPT" } }""")
            .With("default", """{ "Section": { "Key1": "Text1" } }""");

        var logger = new InMemoryLogger<JsonTranslator>();
        var sut = CreateSut(fileProvider, logger);
        sut.SetCulture("pt-BR");

        // act
        var result = sut["Section_Key1"];

        // assert: falls back to "pt". With AllowDuplicateProperties
        // disabled, this is caught as a JsonException and logged at
        // Error — same route a malformed file already takes.
        Assert.Equal("TextoPT", result);
        logger.VerifyError();
    }

    [Fact]
    public void Indexer_WhenComposedKeyCollidesAcrossDifferentSections_LogsAndFallsBackToNeutralOrDefaultFile() {
        // arrange: "pt-BR" has section "A_B" with key "C" and section "A"
        // with key "B_C" — both compose (TranslationKey.Compose) to the
        // same flat key "A_B_C", because the separator ("_") also appears
        // inside the section/key names themselves. This must fail loudly
        // (logged) and fall back, the same way #197's duplicate-key rule
        // does, instead of letting whichever section is enumerated last
        // silently win.
        var fileProvider = new FakeTranslationFileProvider()
            .With("pt-BR", """
                {
                    "A_B": { "C": "First" },
                    "A": { "B_C": "Second" }
                }
                """)
            .With("pt", """{ "Section": { "Key1": "TextoPT" } }""")
            .With("default", """{ "Section": { "Key1": "Text1" } }""");

        var logger = new InMemoryLogger<JsonTranslator>();
        var sut = CreateSut(fileProvider, logger);
        sut.SetCulture("pt-BR");

        // act
        var result = sut["Section_Key1"];

        // assert: falls back to "pt", and the collision was actually
        // logged, not silently resolved to whichever section won.
        Assert.Equal("TextoPT", result);
        logger.VerifyWarning(message => message.Contains("A_B_C", StringComparison.Ordinal));
    }

    [Theory]
    [InlineData("""{ "": { "Key1": "First" } }""")]
    [InlineData("""{ "   ": { "Key1": "First" } }""")]
    public void Indexer_WhenSectionNameIsEmptyOrWhiteSpace_LogsAndFallsBackToNeutralOrDefaultFile(string malformedJson) {
        // arrange: an empty or white-space section name is a shape
        // mismatch — TranslationKey.Compose has nothing meaningful to
        // compose with, so this must fail loudly (logged) and fall back,
        // the same way every other shape mismatch this method logs does.
        var fileProvider = new FakeTranslationFileProvider()
            .With("pt-BR", malformedJson)
            .With("pt", """{ "Section": { "Key1": "TextoPT" } }""")
            .With("default", """{ "Section": { "Key1": "Text1" } }""");

        var logger = new InMemoryLogger<JsonTranslator>();
        var sut = CreateSut(fileProvider, logger);
        sut.SetCulture("pt-BR");

        // act
        var result = sut["Section_Key1"];

        // assert
        Assert.Equal("TextoPT", result);
        logger.VerifyWarning();
    }

    [Theory]
    [InlineData("""{ "SectionA": { "": "First" } }""")]
    [InlineData("""{ "SectionA": { "   ": "First" } }""")]
    public void Indexer_WhenEntryKeyIsEmptyOrWhiteSpace_LogsAndFallsBackToNeutralOrDefaultFile(string malformedJson) {
        // arrange: an empty or white-space entry key is a shape mismatch
        // for the same reason as an empty section name above.
        var fileProvider = new FakeTranslationFileProvider()
            .With("pt-BR", malformedJson)
            .With("pt", """{ "Section": { "Key1": "TextoPT" } }""")
            .With("default", """{ "Section": { "Key1": "Text1" } }""");

        var logger = new InMemoryLogger<JsonTranslator>();
        var sut = CreateSut(fileProvider, logger);
        sut.SetCulture("pt-BR");

        // act
        var result = sut["Section_Key1"];

        // assert
        Assert.Equal("TextoPT", result);
        logger.VerifyWarning(message => message.Contains("SectionA", StringComparison.Ordinal));
    }

    [Fact]
    public void Indexer_WhenSectionNameDuplicatedAtTopLevel_LogsAndFallsBackToNeutralOrDefaultFile() {
        // arrange: "pt-BR" declares "SectionA" twice at the top level —
        // default JsonSerializerOptions would silently drop the first
        // block entirely with nothing logged, so this must fail loudly
        // (logged) and fall back instead.
        var fileProvider = new FakeTranslationFileProvider()
            .With("pt-BR", """
                {
                    "SectionA": { "Key1": "First" },
                    "SectionA": { "Key2": "Second" }
                }
                """)
            .With("pt", """{ "Section": { "Key1": "TextoPT" } }""")
            .With("default", """{ "Section": { "Key1": "Text1" } }""");

        var logger = new InMemoryLogger<JsonTranslator>();
        var sut = CreateSut(fileProvider, logger);
        sut.SetCulture("pt-BR");

        // act
        var result = sut["Section_Key1"];

        // assert: falls back to "pt". With AllowDuplicateProperties
        // disabled, this is caught as a JsonException and logged at
        // Error instead of silently dropping the first "SectionA" block.
        Assert.Equal("TextoPT", result);
        logger.VerifyError();
    }

    [Fact]
    public void Indexer_WhenSectionIsNull_LogsAndFallsBackToNeutralOrDefaultFile() {
        // arrange: "pt-BR" declares "SectionA" as JSON null instead of an
        // object mapping keys to text — this must fail loudly (logged)
        // and fall back, same as every other shape mismatch this method
        // logs, instead of silently treating it as an empty section.
        var fileProvider = new FakeTranslationFileProvider()
            .With("pt-BR", """{ "SectionA": null }""")
            .With("pt", """{ "Section": { "Key1": "TextoPT" } }""")
            .With("default", """{ "Section": { "Key1": "Text1" } }""");

        var logger = new InMemoryLogger<JsonTranslator>();
        var sut = CreateSut(fileProvider, logger);
        sut.SetCulture("pt-BR");

        // act
        var result = sut["Section_Key1"];

        // assert
        Assert.Equal("TextoPT", result);
        logger.VerifyWarning(message => message.Contains("SectionA", StringComparison.Ordinal));
    }

    [Fact]
    public void Indexer_WhenSpecificCultureFileIsMalformedJson_LogsAndFallsBackToNeutralOrDefaultFile() {
        // arrange: pt-BR is malformed (wrong shape — array, not object) —
        // the lookup must not throw and must still resolve "Key1" from
        // "pt" instead of crashing on every binding re-read.
        var fileProvider = new FakeTranslationFileProvider()
            .With("pt-BR", """[ "not", "an", "object" ]""")
            .With("pt", """{ "Section": { "Key1": "TextoPT" } }""")
            .With("default", """{ "Section": { "Key1": "Text1" } }""");

        var sut = CreateSut(fileProvider);
        sut.SetCulture("pt-BR");

        // act
        var result = sut["Section_Key1"];

        // assert
        Assert.Equal("TextoPT", result);
    }

    [Fact]
    public void Indexer_WhenSpecificCultureFileMissing_FallsBackToNeutralCultureFile() {
        // arrange: no "pt-BR" file, but "pt" and "default" exist.
        var fileProvider = new FakeTranslationFileProvider()
            .With("pt", """{ "Section": { "Key1": "TextoPT" } }""")
            .With("default", """{ "Section": { "Key1": "Text1" } }""");

        var sut = CreateSut(fileProvider);
        sut.SetCulture("pt-BR");

        // act
        var result = sut["Section_Key1"];

        // assert: value came from "pt", not "default".
        Assert.Equal("TextoPT", result);
    }

    [Fact]
    public void Indexer_WhenNoCultureFilesExist_FallsBackToDefaultFile() {
        // arrange: neither "pt-BR" nor "pt" exist.
        var fileProvider = new FakeTranslationFileProvider()
            .With("default", """{ "Section": { "Key1": "Text1" } }""");

        var sut = CreateSut(fileProvider);
        sut.SetCulture("pt-BR");

        // act
        var result = sut["Section_Key1"];

        // assert
        Assert.Equal("Text1", result);
    }

    [Fact]
    public void Indexer_WhenKeyAbsentFromResolvedFile_ReturnsBracketedKey() {
        // arrange
        var fileProvider = new FakeTranslationFileProvider()
            .With("default", """{ "Section": { "Key1": "Text1" } }""");

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
            .With("default", """{ "Section": { "Greeting": "Hello, {0}" } }""");

        var sut = CreateSut(fileProvider);

        // act
        var result = sut["Section_Greeting", "Marco"];

        // assert
        Assert.Equal("Hello, Marco", result);
    }

    [Fact]
    public void Indexer_WithParameters_WhenPlaceholderUnsatisfied_ThrowsFormatException() {
        // arrange
        var fileProvider = new FakeTranslationFileProvider()
            .With("default", """{ "Section": { "Greeting": "Hello, {0}" } }""");

        var sut = CreateSut(fileProvider);

        // act & assert: matches ResourceTranslator's current, uncaught behavior.
        Assert.Throws<FormatException>(() => sut["Section_Greeting"]);
    }

    [Fact]
    public void SetCulture_InvalidatesCachedTranslations_SoTheNextLookupResolvesAgainstTheNewCulture() {
        // arrange
        var fileProvider = new FakeTranslationFileProvider()
            .With("pt", """{ "Section": { "Key1": "TextoPT" } }""")
            .With("default", """{ "Section": { "Key1": "Text1" } }""");

        var sut = CreateSut(fileProvider);

        // act: normalize to a culture with no matching file (falls back
        // to "default"), resolving + caching against it first — ensures
        // the environment's own current UI culture cannot make this a
        // no-op.
        sut.SetCulture("en-US");
        _ = sut["Section_Key1"];

        sut.SetCulture("pt-BR");
        var result = sut["Section_Key1"];

        // assert
        Assert.Equal("TextoPT", result);
    }

    [Fact]
    public void SetCulture_RaisesIndexerPropertyChanged_SoBoundControlsRefresh() {
        // arrange
        var fileProvider = new FakeTranslationFileProvider()
            .With("default", """{ "Section": { "Key1": "Text1" } }""");

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

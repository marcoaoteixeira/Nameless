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
            .With("default", """{ "Section": [ { "Key1": "Text1" } ] }""");

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
            .With("pt-BR", """{ "Section": [ { "Key1": "TextoPT-BR" } ] }""")
            .With("pt", """{ "Section": [ { "Key1": "TextoPT" } ] }""")
            .With("default", """{ "Section": [ { "Key1": "Text1" } ] }""");

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
            .With("pt-BR", """{ "Section": [ { "Key1": "TextoPT-BR" } ] }""")
            .With("pt", """{ "Section": [ { "Key1": "TextoPT", "Key2": "TextoPT2" } ] }""")
            .With("default", """{ "Section": [ { "Key1": "Text1", "Key2": "Text2", "Key3": "Text3" } ] }""");

        var sut = CreateSut(fileProvider);
        sut.SetCulture("pt-BR");

        // act & assert
        Assert.Equal("TextoPT-BR", sut["Key1"]);
        Assert.Equal("TextoPT2", sut["Key2"]);
        Assert.Equal("Text3", sut["Key3"]);
    }

    [Fact]
    public void Indexer_WhenKeysAreGroupedIntoDifferentSections_BothSectionsResolveByTheirOwnUnprefixedKey() {
        // arrange: the on-disk grouping is a purely organizational
        // concern — the lookup key is never prefixed with its section
        // name, so two different sections' keys coexist in the flat
        // lookup as long as the keys themselves are unique.
        var fileProvider = new FakeTranslationFileProvider()
            .With("default", """
                {
                    "SectionA": [ { "Key1": "FromSectionA" } ],
                    "SectionB": [ { "Key2": "FromSectionB" } ]
                }
                """);

        var sut = CreateSut(fileProvider);

        // act & assert
        Assert.Equal("FromSectionA", sut["Key1"]);
        Assert.Equal("FromSectionB", sut["Key2"]);
    }

    [Fact]
    public void Indexer_WhenKeyDuplicatedWithinTheSameSection_LogsAndFallsBackToNeutralOrDefaultFile() {
        // arrange: "pt-BR" repeats "Key1" within "SectionA" itself — this
        // must fail loudly (logged) and fall back, exactly like a
        // malformed file, instead of silently picking one of the values.
        var fileProvider = new FakeTranslationFileProvider()
            .With("pt-BR", """
                {
                    "SectionA": [
                        { "Key1": "First" },
                        { "Key1": "Second" }
                    ]
                }
                """)
            .With("pt", """{ "Section": [ { "Key1": "TextoPT" } ] }""")
            .With("default", """{ "Section": [ { "Key1": "Text1" } ] }""");

        var logger = new InMemoryLogger<JsonTranslator>();
        var sut = CreateSut(fileProvider, logger);
        sut.SetCulture("pt-BR");

        // act
        var result = sut["Key1"];

        // assert: falls back to "pt", and the duplicate was actually
        // logged, not silently resolved.
        Assert.Equal("TextoPT", result);
        logger.VerifyWarning(message => message.Contains("Key1", StringComparison.Ordinal)
                                      && message.Contains("SectionA", StringComparison.Ordinal));
    }

    [Fact]
    public void Indexer_WhenKeyDuplicatedWithinTheSameEntryObject_LogsAndFallsBackToNeutralOrDefaultFile() {
        // arrange: "pt-BR" repeats "Key1" within the *same* entry object —
        // default JsonSerializerOptions would silently keep the last
        // value ("Second") without ever reaching the seenInSection check,
        // so this must fail loudly (logged) and fall back instead.
        var fileProvider = new FakeTranslationFileProvider()
            .With("pt-BR", """{ "SectionA": [ { "Key1": "First", "Key1": "Second" } ] }""")
            .With("pt", """{ "Section": [ { "Key1": "TextoPT" } ] }""")
            .With("default", """{ "Section": [ { "Key1": "Text1" } ] }""");

        var logger = new InMemoryLogger<JsonTranslator>();
        var sut = CreateSut(fileProvider, logger);
        sut.SetCulture("pt-BR");

        // act
        var result = sut["Key1"];

        // assert: falls back to "pt". With AllowDuplicateProperties
        // disabled, this is caught as a JsonException and logged at
        // Error (not reaching the seenInSection warning path at all) —
        // same route a malformed file already takes.
        Assert.Equal("TextoPT", result);
        logger.VerifyError();
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
                    "SectionA": [ { "Key1": "First" } ],
                    "SectionA": [ { "Key2": "Second" } ]
                }
                """)
            .With("pt", """{ "Section": [ { "Key1": "TextoPT" } ] }""")
            .With("default", """{ "Section": [ { "Key1": "Text1" } ] }""");

        var logger = new InMemoryLogger<JsonTranslator>();
        var sut = CreateSut(fileProvider, logger);
        sut.SetCulture("pt-BR");

        // act
        var result = sut["Key1"];

        // assert: falls back to "pt". With AllowDuplicateProperties
        // disabled, this is caught as a JsonException and logged at
        // Error instead of silently dropping the first "SectionA" block.
        Assert.Equal("TextoPT", result);
        logger.VerifyError();
    }

    [Fact]
    public void Indexer_WhenSectionIsNull_LogsAndFallsBackToNeutralOrDefaultFile() {
        // arrange: "pt-BR" declares "SectionA" as JSON null instead of an
        // array of entry objects — this must fail loudly (logged) and
        // fall back, same as every other shape mismatch this method logs,
        // instead of silently treating it as an empty section.
        var fileProvider = new FakeTranslationFileProvider()
            .With("pt-BR", """{ "SectionA": null }""")
            .With("pt", """{ "Section": [ { "Key1": "TextoPT" } ] }""")
            .With("default", """{ "Section": [ { "Key1": "Text1" } ] }""");

        var logger = new InMemoryLogger<JsonTranslator>();
        var sut = CreateSut(fileProvider, logger);
        sut.SetCulture("pt-BR");

        // act
        var result = sut["Key1"];

        // assert
        Assert.Equal("TextoPT", result);
        logger.VerifyWarning(message => message.Contains("SectionA", StringComparison.Ordinal));
    }

    [Fact]
    public void Indexer_WhenEntryIsNull_LogsAndFallsBackToNeutralOrDefaultFile() {
        // arrange: "pt-BR" declares an entry inside "SectionA" as JSON
        // null instead of an object — this must fail loudly (logged) and
        // fall back instead of silently treating it as an empty entry.
        var fileProvider = new FakeTranslationFileProvider()
            .With("pt-BR", """{ "SectionA": [ null ] }""")
            .With("pt", """{ "Section": [ { "Key1": "TextoPT" } ] }""")
            .With("default", """{ "Section": [ { "Key1": "Text1" } ] }""");

        var logger = new InMemoryLogger<JsonTranslator>();
        var sut = CreateSut(fileProvider, logger);
        sut.SetCulture("pt-BR");

        // act
        var result = sut["Key1"];

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
            .With("pt", """{ "Section": [ { "Key1": "TextoPT" } ] }""")
            .With("default", """{ "Section": [ { "Key1": "Text1" } ] }""");

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
            .With("pt", """{ "Section": [ { "Key1": "TextoPT" } ] }""")
            .With("default", """{ "Section": [ { "Key1": "Text1" } ] }""");

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
            .With("default", """{ "Section": [ { "Key1": "Text1" } ] }""");

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
            .With("default", """{ "Section": [ { "Key1": "Text1" } ] }""");

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
            .With("default", """{ "Section": [ { "Greeting": "Hello, {0}" } ] }""");

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
            .With("default", """{ "Section": [ { "Greeting": "Hello, {0}" } ] }""");

        var sut = CreateSut(fileProvider);

        // act & assert: matches ResourceTranslator's current, uncaught behavior.
        Assert.Throws<FormatException>(() => sut["Greeting"]);
    }

    [Fact]
    public void SetCulture_InvalidatesCachedTranslations_SoTheNextLookupResolvesAgainstTheNewCulture() {
        // arrange
        var fileProvider = new FakeTranslationFileProvider()
            .With("pt", """{ "Section": [ { "Key1": "TextoPT" } ] }""")
            .With("default", """{ "Section": [ { "Key1": "Text1" } ] }""");

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
            .With("default", """{ "Section": [ { "Key1": "Text1" } ] }""");

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

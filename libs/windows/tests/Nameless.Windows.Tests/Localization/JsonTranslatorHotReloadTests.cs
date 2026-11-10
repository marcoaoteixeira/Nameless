using System.Diagnostics.CodeAnalysis;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Logging;
using Nameless.Windows.Localization.Json;

namespace Nameless.Windows.Localization;

/// <summary>
///     Pins <see cref="JsonTranslator"/>'s reaction to
///     <see cref="ITranslationFileProvider.Changed"/> — the reload/cache/
///     logging logic a hot-reload-capable provider drives, exercised here
///     against a fake that raises the event directly, with no real
///     <see cref="System.IO.FileSystemWatcher"/> or debounce involved
///     (#191's acceptance criteria).
/// </summary>
[UnitTest]
public class JsonTranslatorHotReloadTests {
    /// <summary>
    ///     In-memory, mutable stand-in for a watching
    ///     <see cref="ITranslationFileProvider"/> — content can be
    ///     replaced after construction and <see cref="Raise"/> simulates
    ///     the provider noticing the change.
    /// </summary>
    private sealed class FakeTranslationFileProvider : ITranslationFileProvider {
        // A plain Dictionary is not safe for concurrent writes even to
        // different keys (e.g. a resize mid-write) — locked here purely
        // so the concurrency test below exercises JsonTranslator's own
        // synchronization, not an incidental race in this test double.
        private readonly Lock _sync = new();
        private readonly Dictionary<string, string> _files = new(StringComparer.OrdinalIgnoreCase);

        public event EventHandler<TranslationFileChangedEventArgs>? Changed;

        public FakeTranslationFileProvider With(string key, string json) {
            lock (_sync) {
                _files[key] = json;
            }

            return this;
        }

        public FakeTranslationFileProvider WithoutFile(string key) {
            lock (_sync) {
                _files.Remove(key);
            }

            return this;
        }

        public bool TryRead(string key, [NotNullWhen(true)] out string? content) {
            lock (_sync) {
                return _files.TryGetValue(key, out content);
            }
        }

        public void Raise(string key) {
            Changed?.Invoke(this, new TranslationFileChangedEventArgs(key));
        }
    }

    private static JsonTranslator CreateSut(ITranslationFileProvider fileProvider, InMemoryLogger<JsonTranslator>? logger = null) {
        return new JsonTranslator(fileProvider, logger ?? new InMemoryLogger<JsonTranslator>());
    }

    [Fact]
    public void Changed_WithValidContentForCurrentCultureFile_ReloadsAndRaisesIndexerPropertyChanged() {
        // arrange
        var fileProvider = new FakeTranslationFileProvider()
            .With("default", """{ "Key1": "Text1" }""");

        using var sut = CreateSut(fileProvider);

        // Normalizes the starting culture — the environment's current UI
        // culture could otherwise already resolve to "default" by chance.
        sut.SetCulture("en-US");

        Assert.Equal("Text1", sut["Key1"]); // first lookup — caches "default"

        var raised = false;
        sut.PropertyChanged += (_, args) => raised = args.PropertyName == "Item[]";

        // act: edit the file in place and notify, as a real watcher would.
        fileProvider.With("default", """{ "Key1": "Text1 (edited)" }""");
        fileProvider.Raise("default");

        // assert
        Assert.True(raised);
        Assert.Equal("Text1 (edited)", sut["Key1"]);
    }

    [Fact]
    public void Changed_WithMalformedJson_LogsKeepsLastKnownGoodState_AndDoesNotRaisePropertyChanged() {
        // arrange
        var fileProvider = new FakeTranslationFileProvider()
            .With("default", """{ "Key1": "Text1" }""");

        var logger = new InMemoryLogger<JsonTranslator>();
        using var sut = CreateSut(fileProvider, logger);

        sut.SetCulture("en-US");

        Assert.Equal("Text1", sut["Key1"]); // caches the known-good "default" entries

        var raised = false;
        sut.PropertyChanged += (_, args) => raised = args.PropertyName == "Item[]";

        // act: the on-disk edit corrupted the file (wrong shape).
        fileProvider.With("default", """[ "not", "an", "object" ]""");
        fileProvider.Raise("default");

        // assert: last known-good content still served, no refresh signal,
        // and the failure is actually logged (not silently swallowed).
        Assert.False(raised);
        Assert.Equal("Text1", sut["Key1"]);
        // A JSON array cannot deserialize into Dictionary<string, string> —
        // System.Text.Json throws JsonException, logged at Error.
        logger.VerifyError(message => message.Contains("default", StringComparison.Ordinal));
    }

    [Fact]
    public void Changed_WithInvalidJsonSyntax_LogsKeepsLastKnownGoodState_AndDoesNotRaisePropertyChanged() {
        // arrange
        var fileProvider = new FakeTranslationFileProvider()
            .With("default", """{ "Key1": "Text1" }""");

        var logger = new InMemoryLogger<JsonTranslator>();
        using var sut = CreateSut(fileProvider, logger);

        sut.SetCulture("en-US");

        Assert.Equal("Text1", sut["Key1"]);

        var raised = false;
        sut.PropertyChanged += (_, args) => raised = args.PropertyName == "Item[]";

        // act: a trailing comma — invalid JSON syntax, not just wrong shape.
        fileProvider.With("default", """{ "Key1": "Text1", }""");
        fileProvider.Raise("default");

        // assert
        Assert.False(raised);
        Assert.Equal("Text1", sut["Key1"]);
        logger.VerifyError(message => message.Contains("default", StringComparison.Ordinal));
    }

    [Fact]
    public void Changed_ForFileOutsideCurrentCultureFallbackChain_EvictsCachedEntry_SoALaterSetCulturePicksUpFreshContent() {
        // arrange: currently resolving against "en-US" → "en" → "default".
        // "pt-BR" is unrelated to what is currently displayed.
        var fileProvider = new FakeTranslationFileProvider()
            .With("default", """{ "Key1": "Text1" }""")
            .With("pt-BR", """{ "Key1": "TextoPT-BR" }""");

        using var sut = CreateSut(fileProvider);

        // Switches into "pt-BR" first and reads a key, so "pt-BR" is
        // actually cached — otherwise evicting it would prove nothing.
        sut.SetCulture("pt-BR");
        Assert.Equal("TextoPT-BR", sut["Key1"]);

        // Switches away: "pt-BR" is now outside the fallback chain.
        sut.SetCulture("en-US");
        Assert.Equal("Text1", sut["Key1"]);

        var raised = false;
        sut.PropertyChanged += (_, args) => raised = args.PropertyName == "Item[]";

        // act: "pt-BR" changes on disk while out of chain — evicted from
        // the cache without a reload or a refresh signal, since nothing
        // currently displayed depends on it.
        fileProvider.With("pt-BR", """{ "Key1": "TextoPT-BR (edited)" }""");
        fileProvider.Raise("pt-BR");

        Assert.False(raised);
        Assert.Equal("Text1", sut["Key1"]);

        // assert: switching back to "pt-BR" now resolves the edited
        // content — proving the stale cache entry was actually evicted,
        // not just ignored.
        sut.SetCulture("pt-BR");
        Assert.Equal("TextoPT-BR (edited)", sut["Key1"]);
    }

    [Fact]
    public void Changed_WhenFileNoLongerReadable_LogsKeepsLastKnownGoodState_AndDoesNotRaisePropertyChanged() {
        // arrange
        var fileProvider = new FakeTranslationFileProvider()
            .With("default", """{ "Key1": "Text1" }""");

        var logger = new InMemoryLogger<JsonTranslator>();
        using var sut = CreateSut(fileProvider, logger);

        sut.SetCulture("en-US");

        Assert.Equal("Text1", sut["Key1"]);

        var raised = false;
        sut.PropertyChanged += (_, args) => raised = args.PropertyName == "Item[]";

        // act: simulates the file being deleted or momentarily unreadable
        // — TryRead now fails for "default" while the cached entries
        // remain untouched.
        fileProvider.WithoutFile("default");
        fileProvider.Raise("default");

        // assert
        Assert.False(raised);
        Assert.Equal("Text1", sut["Key1"]);
        logger.VerifyWarning(message => message.Contains("default", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Changed_RaisedConcurrentlyForTwoDifferentFiles_LeavesCacheConsistent_AndBothReloadsVisible() {
        // arrange: "en-US" -> "en" -> "default" is the fallback chain for
        // "en-US" — both "en-US" and "en" are in it at once, so a
        // concurrent reload of either is exercising the same _fileCache
        // and _translations fields OnFileChanged mutates without any
        // synchronization before this fix.
        var fileProvider = new FakeTranslationFileProvider()
            .With("en-US", """{ "Key1": "Specific" }""")
            .With("en", """{ "Key1": "Neutral", "Key2": "NeutralOnly" }""")
            .With("default", """{ "Key1": "Default" }""");

        using var sut = CreateSut(fileProvider);

        sut.SetCulture("en-US");

        // Primes both files into the cache before the concurrent edits.
        Assert.Equal("Specific", sut["Key1"]);
        Assert.Equal("NeutralOnly", sut["Key2"]);

        using var start = new Barrier(2);

        void EditAndRaise(string key, string json) {
            start.SignalAndWait();
            fileProvider.With(key, json);
            fileProvider.Raise(key);
        }

        var cancellationToken = TestContext.Current.CancellationToken;
        var taskA = Task.Run(() => EditAndRaise("en-US", """{ "Key1": "Specific (edited)" }"""), cancellationToken);
        var taskB = Task.Run(() => EditAndRaise("en", """{ "Key1": "Neutral (edited)", "Key2": "NeutralOnly (edited)" }"""), cancellationToken);

        await Task.WhenAll(taskA, taskB);

        // assert: both reloads landed — no lost update, no corrupted
        // dictionary — regardless of which thread's lock section ran
        // first. "en-US" still wins over "en" for "Key1" (fallback order
        // preserved); "Key2" (only defined in "en") reflects its own edit.
        Assert.Equal("Specific (edited)", sut["Key1"]);
        Assert.Equal("NeutralOnly (edited)", sut["Key2"]);
    }

    [Fact]
    public void Dispose_UnsubscribesFromProviderChanged_SoFurtherRaisesHaveNoEffect() {
        // arrange
        var fileProvider = new FakeTranslationFileProvider()
            .With("default", """{ "Key1": "Text1" }""");

        var sut = CreateSut(fileProvider);

        sut.SetCulture("en-US");

        Assert.Equal("Text1", sut["Key1"]);

        var raised = false;
        sut.PropertyChanged += (_, args) => raised = args.PropertyName == "Item[]";

        // act
        sut.Dispose();

        fileProvider.With("default", """{ "Key1": "Text1 (edited)" }""");
        fileProvider.Raise("default");

        // assert: disposed translator no longer reacts.
        Assert.False(raised);
    }
}

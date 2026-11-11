using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Logging;
using Nameless.Windows.Localization.Json;
using SysDirectory = System.IO.Directory;
using SysFile = System.IO.File;
using SysPath = System.IO.Path;

namespace Nameless.Windows.Localization;

/// <summary>
///     End-to-end proof that an on-disk edit reaches
///     <see cref="JsonTranslator"/> through a real
///     <see cref="System.IO.FileSystemWatcher"/>-backed
///     <see cref="FileSystemTranslationFileProvider"/>, with no fake in
///     between — #191's acceptance criteria 1 and 2, plus #192's real-file
///     integration scenarios (startup read, invalid edit, debounced rapid
///     edits). Kept to the minimum needed to prove the wiring: the
///     reload/cache/logging decision logic itself is covered in depth,
///     deterministically, by <c>JsonTranslatorHotReloadTests</c> against a
///     fake provider.
/// </summary>
/// <remarks>
///     Written defensively against real <see cref="System.IO.FileSystemWatcher"/>
///     flakiness (per the repo's own #179 lesson): a generous polling wait
///     instead of a fixed delay, and a debounce window wide enough that
///     back-to-back in-process file writes still land inside it.
/// </remarks>
[IntegrationTest]
public sealed class JsonTranslatorFileSystemWatcherIntegrationTests : IDisposable {
    private readonly string _rootPath;

    public JsonTranslatorFileSystemWatcherIntegrationTests() {
        _rootPath = SysPath.Combine(SysPath.GetTempPath(), $"nameless-translator-e2e-tests-{Guid.NewGuid():N}");

        SysDirectory.CreateDirectory(_rootPath);
    }

    public void Dispose() {
        SysDirectory.Delete(_rootPath, recursive: true);
    }

    private static bool WaitFor(Func<bool> condition, int timeoutMilliseconds = 5_000) {
        var deadline = DateTime.UtcNow.AddMilliseconds(timeoutMilliseconds);
        while (DateTime.UtcNow < deadline) {
            if (condition()) {
                return true;
            }

            Thread.Sleep(25);
        }

        return condition();
    }

    [Fact]
    public void EditingTheCurrentCultureFileOnDisk_RefreshesJsonTranslatorLookupsAndRaisesIndexerPropertyChanged_WithNoRestart() {
        // arrange
        SysFile.WriteAllText(SysPath.Combine(_rootPath, "default.json"), """{ "Section": [ { "Key1": "Text1" } ] }""");

        using var fileProvider = new FileSystemTranslationFileProvider(
            Options.Create(new TranslatorOptions { RootPath = _rootPath, WatcherDebounceDelay = TimeSpan.FromMilliseconds(100) }),
            NullLogger<FileSystemTranslationFileProvider>.Instance
        );

        using var sut = new JsonTranslator(fileProvider, NullLogger<JsonTranslator>.Instance);

        sut.SetCulture("en-US"); // normalizes the starting culture

        Assert.Equal("Text1", sut["Section_Key1"]); // first lookup — caches "default"

        var raised = false;
        sut.PropertyChanged += (_, args) => raised = raised || args.PropertyName == "Item[]";

        // act: a non-developer edits the file while the app is running.
        SysFile.WriteAllText(SysPath.Combine(_rootPath, "default.json"), """{ "Section": [ { "Key1": "Text1 (edited)" } ] }""");

        var delivered = WaitFor(() => sut["Section_Key1"] == "Text1 (edited)");

        // assert
        Assert.True(delivered, "Expected the edited value to be visible through the indexer within the timeout.");
        Assert.True(raised, "Expected the 'Item[]' PropertyChanged signal to have fired for bound controls to refresh.");
    }

    [Fact]
    public void Construction_WithNoPriorStateOrEdits_ReadsTheRealFileFromDiskAndResolvesTheKey() {
        // arrange: #192 scenario 1 — a plain startup read, explicitly
        // without any hot-reload/edit involved, against a real provider
        // and real JsonTranslator reading a real file from disk.
        SysFile.WriteAllText(SysPath.Combine(_rootPath, "default.json"), """{ "Section": [ { "Key1": "Text1" } ] }""");

        using var fileProvider = new FileSystemTranslationFileProvider(
            Options.Create(new TranslatorOptions { RootPath = _rootPath }),
            NullLogger<FileSystemTranslationFileProvider>.Instance
        );

        using var sut = new JsonTranslator(fileProvider, NullLogger<JsonTranslator>.Instance);

        sut.SetCulture("en-US");

        // act & assert
        Assert.Equal("Text1", sut["Section_Key1"]);
    }

    [Fact]
    public void EditingTheCurrentCultureFileWithInvalidJson_LogsKeepsLastKnownGoodState_AndDoesNotCrash() {
        // arrange: #192 scenario 3 — against real files and a real
        // FileSystemWatcher (not the fake provider used by
        // JsonTranslatorHotReloadTests), an invalid on-disk edit must be
        // logged and ignored, with the previous state still served.
        SysFile.WriteAllText(SysPath.Combine(_rootPath, "default.json"), """{ "Section": [ { "Key1": "Text1" } ] }""");

        using var fileProvider = new FileSystemTranslationFileProvider(
            Options.Create(new TranslatorOptions { RootPath = _rootPath, WatcherDebounceDelay = TimeSpan.FromMilliseconds(100) }),
            NullLogger<FileSystemTranslationFileProvider>.Instance
        );

        var logger = new InMemoryLogger<JsonTranslator>();
        using var sut = new JsonTranslator(fileProvider, logger);

        sut.SetCulture("en-US");

        Assert.Equal("Text1", sut["Section_Key1"]); // first lookup — caches "default"

        var raised = false;
        sut.PropertyChanged += (_, args) => raised = raised || args.PropertyName == "Item[]";

        // act: the on-disk edit corrupts the file (invalid JSON syntax).
        SysFile.WriteAllText(SysPath.Combine(_rootPath, "default.json"), """{ "Key1": "Text1", }""");

        // Waits out a generous window to let a real, asynchronous reload
        // attempt (and its logging) actually happen.
        Assert.True(WaitFor(() => logger.Entries.Count > 0, timeoutMilliseconds: 5_000),
            "Expected the invalid edit to be logged within the timeout.");

        // assert: no refresh signal fired for the bad edit itself, and the
        // failure was actually logged — not silently swallowed.
        Assert.False(raised);
        logger.VerifyError(message => message.Contains("default", StringComparison.Ordinal));

        // Forces JsonTranslator to re-merge from its per-file cache (a
        // culture round-trip legitimately raises its own "Item[]" signal,
        // unrelated to the bad edit, so it's done after the asserts above),
        // so this final assertion actually exercises the merged
        // _translations state rather than only proving the per-file cache
        // entry survived untouched.
        sut.SetCulture("pt-BR");
        sut.SetCulture("en-US");

        Assert.Equal("Text1", sut["Section_Key1"]);
    }

    [Fact]
    public void RapidSuccessiveEditsToTheCurrentCultureFile_AreDebouncedToASingleReload() {
        // arrange: #192 scenario 4 — against real files and a real
        // FileSystemWatcher, proven at the full JsonTranslator/indexer
        // level (one "Item[]" signal, final correct value), not just at
        // the provider's Changed-event count already covered by
        // FileSystemTranslationFileProviderTests.
        SysFile.WriteAllText(SysPath.Combine(_rootPath, "default.json"), """{ "Section": [ { "Key1": "Text1" } ] }""");

        using var fileProvider = new FileSystemTranslationFileProvider(
            Options.Create(new TranslatorOptions { RootPath = _rootPath, WatcherDebounceDelay = TimeSpan.FromMilliseconds(400) }),
            NullLogger<FileSystemTranslationFileProvider>.Instance
        );

        using var sut = new JsonTranslator(fileProvider, NullLogger<JsonTranslator>.Instance);

        sut.SetCulture("en-US");

        Assert.Equal("Text1", sut["Section_Key1"]); // first lookup — caches "default"

        var raisedCount = 0;
        sut.PropertyChanged += (_, args) => {
            if (args.PropertyName == "Item[]") {
                Interlocked.Increment(ref raisedCount);
            }
        };

        // act: two rapid successive edits, well inside the debounce window.
        SysFile.WriteAllText(SysPath.Combine(_rootPath, "default.json"), """{ "Section": [ { "Key1": "Text1 (edit 1)" } ] }""");
        Thread.Sleep(50);
        SysFile.WriteAllText(SysPath.Combine(_rootPath, "default.json"), """{ "Section": [ { "Key1": "Text1 (edit 2)" } ] }""");

        var delivered = WaitFor(() => sut["Section_Key1"] == "Text1 (edit 2)");

        // Holds for a bit longer past the debounce window to make sure a
        // second, unwanted reload doesn't show up late.
        Thread.Sleep(600);

        // assert
        Assert.True(delivered, "Expected the latest edited value to be visible through the indexer within the timeout.");
        Assert.Equal("Text1 (edit 2)", sut["Section_Key1"]);
        Assert.Equal(1, raisedCount);
    }
}

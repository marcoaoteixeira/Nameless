using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Nameless.Testing.Tools.Attributes;
using Nameless.Windows.Localization;
using Nameless.Windows.Localization.Json;
using SysDirectory = System.IO.Directory;
using SysFile = System.IO.File;
using SysPath = System.IO.Path;

namespace Nameless.Windows.Tests.Localization;

/// <summary>
///     End-to-end proof that an on-disk edit reaches
///     <see cref="JsonTranslator"/> through a real
///     <see cref="System.IO.FileSystemWatcher"/>-backed
///     <see cref="FileSystemTranslationFileProvider"/>, with no fake in
///     between — #191's acceptance criteria 1 and 2. Kept to the minimum
///     needed to prove the wiring: the reload/cache/logging decision logic
///     itself is covered in depth, deterministically, by
///     <c>JsonTranslatorHotReloadTests</c> against a fake provider.
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
        SysFile.WriteAllText(SysPath.Combine(_rootPath, "default.json"), """{ "Key1": "Text1" }""");

        using var fileProvider = new FileSystemTranslationFileProvider(
            Options.Create(new TranslatorOptions { RootPath = _rootPath, WatcherDebounceDelay = TimeSpan.FromMilliseconds(100) }),
            NullLogger<FileSystemTranslationFileProvider>.Instance
        );

        using var sut = new JsonTranslator(fileProvider, NullLogger<JsonTranslator>.Instance);

        sut.SetCulture("en-US"); // normalizes the starting culture

        Assert.Equal("Text1", sut["Key1"]); // first lookup — caches "default"

        var raised = false;
        sut.PropertyChanged += (_, args) => raised = raised || args.PropertyName == "Item[]";

        // act: a non-developer edits the file while the app is running.
        SysFile.WriteAllText(SysPath.Combine(_rootPath, "default.json"), """{ "Key1": "Text1 (edited)" }""");

        var delivered = WaitFor(() => sut["Key1"] == "Text1 (edited)");

        // assert
        Assert.True(delivered, "Expected the edited value to be visible through the indexer within the timeout.");
        Assert.True(raised, "Expected the 'Item[]' PropertyChanged signal to have fired for bound controls to refresh.");
    }
}

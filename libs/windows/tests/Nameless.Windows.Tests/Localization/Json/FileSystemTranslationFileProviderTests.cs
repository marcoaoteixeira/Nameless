using System.IO;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Nameless.Configuration;
using Nameless.Testing.Tools.Attributes;
using SysDirectory = System.IO.Directory;
using SysFile = System.IO.File;
using SysPath = System.IO.Path;

namespace Nameless.Windows.Localization.Json;

/// <summary>
///     Pins <see cref="FileSystemTranslationFileProvider"/>'s real disk
///     reads — the seam <see cref="Nameless.Windows.Localization.JsonTranslator"/>
///     is resolved against once DI is wired (#190's acceptance criteria).
/// </summary>
[IntegrationTest]
public sealed class FileSystemTranslationFileProviderTests : IDisposable {
    private readonly string _rootPath;

    public FileSystemTranslationFileProviderTests() {
        _rootPath = SysPath.Combine(SysPath.GetTempPath(), $"nameless-translator-tests-{Guid.NewGuid():N}");

        SysDirectory.CreateDirectory(_rootPath);
    }

    public void Dispose() {
        SysDirectory.Delete(_rootPath, recursive: true);
    }

    private static FileSystemTranslationFileProvider CreateSut(string? rootPath, TimeSpan? watcherDebounceDelay = null) {
        return new FileSystemTranslationFileProvider(
            Options.Create(new TranslatorOptions {
                RootPath = rootPath,
                WatcherDebounceDelay = watcherDebounceDelay ?? TimeSpan.FromMilliseconds(50)
            }),
            NullLogger<FileSystemTranslationFileProvider>.Instance
        );
    }

    // Generous relative to the 50ms debounce used above — real
    // FileSystemWatcher delivery has OS-level jitter (the repo's own
    // known flaky-CI-timing lesson, #179), so this waits up to a few
    // seconds rather than asserting on a tight deadline.
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
    public void TryRead_WhenDefaultFileExists_ReturnsTrueAndItsContent() {
        // arrange
        SysFile.WriteAllText(SysPath.Combine(_rootPath, "default.json"), """{ "Key1": "Text1" }""");

        using var sut = CreateSut(_rootPath);

        // act
        var found = sut.TryRead("default", out var content);

        // assert
        Assert.True(found);
        Assert.Equal("""{ "Key1": "Text1" }""", content);
    }

    [Fact]
    public void TryRead_WhenCultureFileExists_ReadsCultureDotJson() {
        // arrange
        SysFile.WriteAllText(SysPath.Combine(_rootPath, "pt-BR.json"), """{ "Key1": "TextoPT-BR" }""");

        using var sut = CreateSut(_rootPath);

        // act
        var found = sut.TryRead("pt-BR", out var content);

        // assert
        Assert.True(found);
        Assert.Equal("""{ "Key1": "TextoPT-BR" }""", content);
    }

    [Fact]
    public void TryRead_WhenFileDoesNotExist_ReturnsFalse() {
        // arrange
        using var sut = CreateSut(_rootPath);

        // act
        var found = sut.TryRead("pt-BR", out var content);

        // assert
        Assert.False(found);
        Assert.Null(content);
    }

    [Fact]
    public void TryRead_WithRelativeRootPath_ResolvesAgainstApplicationBaseDirectory() {
        // arrange: a relative RootPath must resolve the same way
        // TranslatorOptions.RootPath is documented to (relative to the
        // application's base directory), matching the sample app's
        // "Resources\\loc" configuration.
        var relativeRootPath = $"nameless-translator-tests-{Guid.NewGuid():N}";
        var absoluteRootPath = SysPath.Combine(AppContext.BaseDirectory, relativeRootPath);

        SysDirectory.CreateDirectory(absoluteRootPath);
        try {
            SysFile.WriteAllText(SysPath.Combine(absoluteRootPath, "default.json"), """{ "Key1": "Text1" }""");

            using var sut = CreateSut(relativeRootPath);

            // act
            var found = sut.TryRead("default", out var content);

            // assert
            Assert.True(found);
            Assert.Equal("""{ "Key1": "Text1" }""", content);
        }
        finally {
            SysDirectory.Delete(absoluteRootPath, recursive: true);
        }
    }

    [Fact]
    public void Constructor_WhenRootPathIsNotConfigured_ThrowsMissingConfigurationException() {
        Assert.Throws<MissingConfigurationException>(() => CreateSut(rootPath: null));
    }

    [Fact]
    public void TryRead_WhenFileIsLockedByAnotherProcess_ReturnsFalse_InsteadOfThrowing() {
        // arrange: an exclusive lock makes File.ReadAllText throw IOException
        // — JsonTranslator's indexer must not crash for every caller just
        // because one culture's file is momentarily locked/unreadable.
        var path = SysPath.Combine(_rootPath, "default.json");

        SysFile.WriteAllText(path, """{ "Key1": "Text1" }""");

        using var sut = CreateSut(_rootPath);

        using var stream = SysFile.Open(path, FileMode.Open, FileAccess.Read, FileShare.None);

        // act
        var found = sut.TryRead("default", out var content);

        // assert
        Assert.False(found);
        Assert.Null(content);
    }

    [Fact]
    public void Changed_WhenAFileIsEditedOnDisk_IsRaisedForThatFilesKey() {
        // arrange
        var path = SysPath.Combine(_rootPath, "default.json");

        SysFile.WriteAllText(path, """{ "Key1": "Text1" }""");

        using var sut = CreateSut(_rootPath);

        TranslationFileChangedEventArgs? raised = null;
        sut.Changed += (_, args) => raised = args;

        // act
        SysFile.WriteAllText(path, """{ "Key1": "Text1 (edited)" }""");

        // assert: generous wait — real FileSystemWatcher delivery is not
        // instantaneous and has OS-level jitter (see #179's lesson on
        // flaky FS timing).
        var delivered = WaitFor(() => raised is not null);

        Assert.True(delivered, "Expected Changed to be raised for the edited file within the timeout.");
        Assert.Equal("default", raised!.Key);
    }

    [Fact]
    public void Changed_WithTwoRapidSuccessiveEditsToTheSameFile_IsRaisedOnlyOnce() {
        // arrange: a debounce window generous enough that two writes
        // performed back-to-back in-process both land inside it.
        var path = SysPath.Combine(_rootPath, "default.json");

        SysFile.WriteAllText(path, """{ "Key1": "Text1" }""");

        using var sut = CreateSut(_rootPath, watcherDebounceDelay: TimeSpan.FromMilliseconds(400));

        var raisedCount = 0;
        sut.Changed += (_, _) => Interlocked.Increment(ref raisedCount);

        // act: two rapid successive edits, well inside the debounce window.
        SysFile.WriteAllText(path, """{ "Key1": "Text1 (edit 1)" }""");
        Thread.Sleep(50);
        SysFile.WriteAllText(path, """{ "Key1": "Text1 (edit 2)" }""");

        // Waits past the debounce window, then holds for a bit longer to
        // make sure a second, unwanted raise doesn't show up late.
        WaitFor(() => raisedCount > 0, timeoutMilliseconds: 3_000);
        Thread.Sleep(600);

        // assert
        Assert.Equal(1, raisedCount);
    }

    [Fact]
    public void Constructor_WhenRootPathDoesNotExist_DoesNotThrow_AndNeverRaisesChanged() {
        // arrange: a RootPath nobody has populated yet is not fatal —
        // there is simply nothing to watch.
        var missingRootPath = SysPath.Combine(_rootPath, "does-not-exist");

        // act & assert: construction itself must not throw.
        using var sut = CreateSut(missingRootPath);

        var raised = false;
        sut.Changed += (_, _) => raised = true;

        var found = sut.TryRead("default", out var content);

        Assert.False(found);
        Assert.Null(content);

        // A missing root means there is no FileSystemWatcher at all, so
        // creating the directory and dropping a file into it afterward
        // still must never raise Changed for this instance.
        SysDirectory.CreateDirectory(missingRootPath);
        SysFile.WriteAllText(SysPath.Combine(missingRootPath, "default.json"), """{ "Key1": "Text1" }""");

        WaitFor(() => raised, timeoutMilliseconds: 500);

        Assert.False(raised);
    }
}

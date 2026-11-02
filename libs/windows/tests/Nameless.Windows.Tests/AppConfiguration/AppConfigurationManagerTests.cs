using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Nameless.Application;
using Nameless.IO.Physical;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.IO;
using Nameless.Windows.AppConfiguration;
using SysDirectory = System.IO.Directory;
using SysFile = System.IO.File;
using SysPath = System.IO.Path;

namespace Nameless.Windows.Tests.AppConfiguration;

/// <summary>
///     Proves the fix for the shutdown-time clobber bug: after an external
///     process (e.g. a restore feature) replaces <c>app.config</c> on
///     disk, <see cref="IAppConfigurationManager.Reload"/> must make this
///     manager's in-memory state match the file on disk, so a subsequent
///     <see cref="IAppConfigurationManager.SaveChangesAsync"/> — the one
///     the WPF host runs on <c>OnExit</c> — writes that content back
///     instead of overwriting it with stale, pre-reload values.
/// </summary>
[UnitTest]
public class AppConfigurationManagerTests {
    private sealed class Sut : IDisposable {
        public required TempDirectory Root { get; init; }
        public required PhysicalFileManager FileManager { get; init; }
        public required AppConfigurationManager Manager { get; init; }

        public void Dispose() => Root.Dispose();
    }

    private static Sut CreateSut() {
        var root = new TempDirectory(nameof(AppConfigurationManagerTests));
        var fileManager = new PhysicalFileManager(root.Path);

        var applicationContextMock = new Mock<IApplicationContext>();
        applicationContextMock.SetupGet(x => x.FileManager).Returns(fileManager);

        var manager = new AppConfigurationManager(
            applicationContextMock.Object,
            NullLogger<AppConfigurationManager>.Instance
        );

        return new Sut { Root = root, FileManager = fileManager, Manager = manager };
    }

    [Fact]
    public void Reload_ReplacesInMemoryValuesWithWhatIsCurrentlyOnDisk() {
        // arrange
        using var sut = CreateSut();

        sut.Manager.Set("Theme", "Dark");

        // Simulate an external swap of the backing file (e.g. a restore)
        // while the value above is still only held in memory.
        var configPath = SysPath.Combine(sut.Root.Path, "app.config");
        SysFile.WriteAllText(configPath, """{"Theme":"Light"}""");

        // act
        sut.Manager.Reload();

        // assert
        Assert.True(sut.Manager.TryGet<string>("Theme", out var theme));
        Assert.Equal("Light", theme);
    }

    [Fact]
    public async Task SaveChangesAsync_AfterReload_PersistsReloadedContent_NotStalePreReloadValues() {
        // arrange
        using var sut = CreateSut();

        // Pre-reload in-memory state: what the long-lived singleton held
        // before the file on disk was swapped out from under it.
        sut.Manager.Set("Theme", "Dark");

        var configPath = SysPath.Combine(sut.Root.Path, "app.config");
        SysFile.WriteAllText(configPath, """{"Theme":"Light"}""");

        sut.Manager.Reload();

        // act: this is the call the WPF host's OnExit makes — proving it no
        // longer clobbers the restored file with the stale "Dark" value.
        await sut.Manager.SaveChangesAsync(TestContext.Current.CancellationToken);

        // assert
        var persisted = SysFile.ReadAllText(configPath);
        Assert.Contains("Light", persisted);
        Assert.DoesNotContain("Dark", persisted);
    }

    [Fact]
    public async Task SaveChangesAsync_WritesAtomically_LeavesNoTempFileAfterSuccess() {
        // arrange: proves the write-then-rename path is actually used —
        // no ".tmp-*" sibling is left behind once the save completes.
        using var sut = CreateSut();

        sut.Manager.Set("Theme", "Dark");

        // act
        await sut.Manager.SaveChangesAsync(TestContext.Current.CancellationToken);

        // assert
        var entries = SysDirectory.GetFiles(sut.Root.Path);
        Assert.DoesNotContain(entries, path => SysPath.GetFileName(path) == "app.config.tmp");
        Assert.Contains(entries, path => SysPath.GetFileName(path) == "app.config");
    }

    [Fact]
    public async Task SaveChangesAsync_WhenWriteFails_NeverLeavesDestinationPartial() {
        // arrange: a reader of app.config must never observe a
        // truncated/partial file. Simulate a write failure (an already
        // cancelled token makes the temp-file write throw before any bytes
        // land) and prove the destination — still holding the previous,
        // fully-written content — was never touched.
        using var sut = CreateSut();

        var configPath = SysPath.Combine(sut.Root.Path, "app.config");
        SysFile.WriteAllText(configPath, """{"Theme":"Light"}""");

        sut.Manager.Set("Theme", "Dark");

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // act
        await sut.Manager.SaveChangesAsync(cts.Token);

        // assert: destination untouched, and the failed temp write did not
        // leave a stray sibling behind either.
        var persisted = SysFile.ReadAllText(configPath);
        Assert.Equal("""{"Theme":"Light"}""", persisted);

        var entries = SysDirectory.GetFiles(sut.Root.Path);
        Assert.DoesNotContain(entries, path => SysPath.GetFileName(path) == "app.config.tmp");
    }

    [Fact]
    public async Task SaveChangesAsync_WhenStaleTempFileExists_OverwritesItAndLeavesNoTempFile() {
        // arrange: an orphan from a previous save that never got renamed
        // away (e.g. the process died between the write and the rename).
        // The fixed temp name must self-heal: the next save's
        // FileMode.Create overwrites it, and the rename still leaves no
        // temp file behind.
        using var sut = CreateSut();

        var configPath = SysPath.Combine(sut.Root.Path, "app.config");
        var tempPath = SysPath.Combine(sut.Root.Path, "app.config.tmp");
        SysFile.WriteAllText(tempPath, "stale junk from a previous failed save");

        sut.Manager.Set("Theme", "Dark");

        // act
        await sut.Manager.SaveChangesAsync(TestContext.Current.CancellationToken);

        // assert
        var persisted = SysFile.ReadAllText(configPath);
        Assert.Contains("Dark", persisted);

        Assert.False(SysFile.Exists(tempPath));
    }

    [Fact]
    public async Task SaveChangesAsync_WhenTempFileIsHeldByAnotherWriter_DoesNotDeleteItAndLeavesDestinationIntact() {
        // arrange: another writer already has the fixed-name temp file
        // open exclusively (FileShare.None). This save's own attempt to
        // open it must fail without ever having created/opened it itself,
        // so the cleanup-on-failure logic must not delete the other
        // writer's in-flight temp file — the `tempCreated` guard.
        using var sut = CreateSut();

        var configPath = SysPath.Combine(sut.Root.Path, "app.config");
        SysFile.WriteAllText(configPath, """{"Theme":"Light"}""");

        var tempPath = SysPath.Combine(sut.Root.Path, "app.config.tmp");

        sut.Manager.Set("Theme", "Dark");

        using var otherWriterLock = new System.IO.FileStream(tempPath, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None);

        // act
        await sut.Manager.SaveChangesAsync(TestContext.Current.CancellationToken);

        // assert: the call did not throw out of SaveChangesAsync (it is
        // logged internally), the held temp file is untouched, and the
        // destination was never overwritten.
        Assert.True(SysFile.Exists(tempPath));

        var persisted = SysFile.ReadAllText(configPath);
        Assert.Equal("""{"Theme":"Light"}""", persisted);
    }

    [Fact]
    public async Task SaveChangesAsync_WithoutReload_WouldClobberAnExternallyReplacedFile() {
        // arrange: documents the bug this feature guards against — without
        // calling Reload() first, the stale in-memory value wins.
        using var sut = CreateSut();

        sut.Manager.Set("Theme", "Dark");

        var configPath = SysPath.Combine(sut.Root.Path, "app.config");
        SysFile.WriteAllText(configPath, """{"Theme":"Light"}""");

        // act: no Reload() call here.
        await sut.Manager.SaveChangesAsync(TestContext.Current.CancellationToken);

        // assert
        var persisted = SysFile.ReadAllText(configPath);
        Assert.Contains("Dark", persisted);
        Assert.DoesNotContain("Light", persisted);
    }
}

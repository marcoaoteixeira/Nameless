using System.IO;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Nameless.Application;
using Nameless.IO.Physical;
using Nameless.Testing.Tools.IO;
using Nameless.WinApp.Tests.Tooling;
using Nameless.Windows;
using Nameless.Windows.Bootstrap.Notification;

namespace Nameless.WinApp.Bootstrap.Steps;

public class CleanupPreviousSystemUpdateStepTests {
    // Plan §5.3, the single most important safety test in the feature: the
    // leftover pattern must match only the exact shapes the handler itself
    // produces, and nothing a real application directory could ever contain.
    [Theory]
    [UnitTest]
    [InlineData("Nameless.WinApp.exe.old-20261103120000", true)]
    [InlineData("appsettings.json.old-20261103120000", true)]
    [InlineData(".update-20261103120000", true)]
    [InlineData(".update-probe-20261103120000", true)]
    [InlineData("logs", false)]
    [InlineData("App_Data", false)]
    [InlineData("Nameless.WinApp.exe", false)]
    [InlineData("appsettings.json", false)]
    [InlineData("foo.old-2026", false)] // wrong digit count (too short)
    [InlineData("foo.old-202611031200000", false)] // wrong digit count (too long)
    [InlineData("foo.old-20261103120000.txt", false)] // extra suffix after the timestamp
    [InlineData(".update-2026110312000", false)] // wrong digit count
    [InlineData("update-20261103120000", false)] // missing the leading dot
    public void IsPreviousUpdateLeftover_MatchesOnlyTheExactLeftoverShapes(string entryName, bool expected) {
        Assert.Equal(expected, CleanupPreviousSystemUpdateStep.IsPreviousUpdateLeftover(entryName));
    }

    private sealed class Sut : IDisposable {
        public required TempDirectory InstallRoot { get; init; }
        public required TempDirectory AppDataRoot { get; init; }
        public required CleanupPreviousSystemUpdateStep Step { get; init; }

        public void Dispose() {
            InstallRoot.Dispose();
            AppDataRoot.Dispose();
        }
    }

    private static Sut CreateSut() {
        var installRoot = new TempDirectory(nameof(CleanupPreviousSystemUpdateStepTests) + "-install");
        var appDataRoot = new TempDirectory(nameof(CleanupPreviousSystemUpdateStepTests) + "-appdata");

        var environment = new FakeSystemUpdateEnvironment {
            InstallDirectoryPath = installRoot.Path,
            ExecutablePath = SysPath.Combine(installRoot.Path, "Nameless.WinApp.exe")
        };

        var fileManager = new PhysicalFileManager(appDataRoot.Path);

        var applicationContextMock = new Mock<IApplicationContext>();
        applicationContextMock.SetupGet(x => x.FileManager).Returns(fileManager);

        var step = new CleanupPreviousSystemUpdateStep(
            environment: environment,
            applicationContext: applicationContextMock.Object,
            translator: new FakeTranslator(),
            logger: NullLogger<CleanupPreviousSystemUpdateStep>.Instance
        );

        return new Sut { InstallRoot = installRoot, AppDataRoot = appDataRoot, Step = step };
    }

    [Fact]
    [UnitTest]
    public async Task ExecuteAsync_DeletesOnlyMatchingLeftoversAndNeverTouchesRuntimeDataOrTheRunningExecutable() {
        using var sut = CreateSut();

        SysDirectory.CreateDirectory(SysPath.Combine(sut.InstallRoot.Path, "logs"));
        SysFile.WriteAllText(SysPath.Combine(sut.InstallRoot.Path, "logs", "app.log"), "keep");

        SysDirectory.CreateDirectory(SysPath.Combine(sut.InstallRoot.Path, "App_Data"));
        SysFile.WriteAllText(SysPath.Combine(sut.InstallRoot.Path, "Nameless.WinApp.exe"), "current-exe");
        SysFile.WriteAllText(SysPath.Combine(sut.InstallRoot.Path, "appsettings.json"), "{}");

        var oldExeSibling = SysPath.Combine(sut.InstallRoot.Path, "Nameless.WinApp.exe.old-20261103120000");
        SysFile.WriteAllText(oldExeSibling, "old-exe");

        var stagingLeftover = SysPath.Combine(sut.InstallRoot.Path, ".update-20261103120000");
        SysDirectory.CreateDirectory(stagingLeftover);
        SysFile.WriteAllText(SysPath.Combine(stagingLeftover, "leftover-file.txt"), "stale");

        var progress = new Progress<StepProgress>();

        await sut.Step.ExecuteAsync(progress, TestContext.Current.CancellationToken);

        var remaining = SysDirectory.GetFileSystemEntries(sut.InstallRoot.Path)
            .Select(SysPath.GetFileName)
            .ToArray();

        Assert.Contains("logs", remaining);
        Assert.Contains("App_Data", remaining);
        Assert.Contains("Nameless.WinApp.exe", remaining);
        Assert.Contains("appsettings.json", remaining);
        Assert.DoesNotContain("Nameless.WinApp.exe.old-20261103120000", remaining);
        Assert.DoesNotContain(".update-20261103120000", remaining);

        Assert.Equal("keep", SysFile.ReadAllText(SysPath.Combine(sut.InstallRoot.Path, "logs", "app.log")));
        Assert.Equal("current-exe", SysFile.ReadAllText(SysPath.Combine(sut.InstallRoot.Path, "Nameless.WinApp.exe")));
    }

    [Fact]
    [UnitTest]
    public async Task ExecuteAsync_DeletesContentsOfTheUpdateDownloadDirectory() {
        using var sut = CreateSut();

        var applicationContextMock = new Mock<IApplicationContext>();
        var fileManager = new PhysicalFileManager(sut.AppDataRoot.Path);
        var updateDirectory = fileManager.GetUpdateDirectory();
        updateDirectory.Create();

        var stalePackagePath = SysPath.Combine(updateDirectory.Path, "Nameless.WinApp-1.0.0-win-x64.zip");
        SysFile.WriteAllText(stalePackagePath, "stale-package");

        var environment = new FakeSystemUpdateEnvironment {
            InstallDirectoryPath = sut.InstallRoot.Path,
            ExecutablePath = SysPath.Combine(sut.InstallRoot.Path, "Nameless.WinApp.exe")
        };

        applicationContextMock.SetupGet(x => x.FileManager).Returns(fileManager);

        var step = new CleanupPreviousSystemUpdateStep(
            environment: environment,
            applicationContext: applicationContextMock.Object,
            translator: new FakeTranslator(),
            logger: NullLogger<CleanupPreviousSystemUpdateStep>.Instance
        );

        await step.ExecuteAsync(new Progress<StepProgress>(), TestContext.Current.CancellationToken);

        Assert.False(SysFile.Exists(stalePackagePath));
    }

    [Fact]
    [UnitTest]
    public async Task ExecuteAsync_WhenTheOldSiblingIsTheOnlySurvivingCopy_KeepsItInstead() {
        using var sut = CreateSut();

        // No "appsettings.json" at its original name — only the
        // ".old-*" sibling survives (e.g. a prior self-heal/rollback
        // failure). Deleting it would leave the install directory with
        // no copy of the file at all.
        var orphanedOldSibling = SysPath.Combine(sut.InstallRoot.Path, "appsettings.json.old-20261103120000");
        SysFile.WriteAllText(orphanedOldSibling, "{\"old\":true}");

        await sut.Step.ExecuteAsync(new Progress<StepProgress>(), TestContext.Current.CancellationToken);

        Assert.True(SysFile.Exists(orphanedOldSibling));
    }

    [Fact]
    [UnitTest]
    public async Task ExecuteAsync_WhenALeftoverIsLocked_SkipsItWithoutThrowing() {
        using var sut = CreateSut();

        var lockedLeftover = SysPath.Combine(sut.InstallRoot.Path, "Nameless.WinApp.exe.old-20261103120000");
        SysFile.WriteAllText(lockedLeftover, "old-exe");

        // The original must exist alongside the leftover, otherwise the
        // "keep the only surviving copy" guard short-circuits before the
        // delete is even attempted, and this test would no longer exercise
        // DeleteBestEffort's locked-file handling.
        SysFile.WriteAllText(SysPath.Combine(sut.InstallRoot.Path, "Nameless.WinApp.exe"), "current-exe");

        using var fileLock = new FileStream(lockedLeftover, FileMode.Open, FileAccess.Read, FileShare.Read);

        await sut.Step.ExecuteAsync(new Progress<StepProgress>(), TestContext.Current.CancellationToken);

        Assert.True(SysFile.Exists(lockedLeftover));
    }
}

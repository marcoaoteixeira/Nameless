using System.Diagnostics;
using System.IO;
using Nameless.Testing.Tools.IO;

namespace Nameless.WinApp.Features.PerformSystemUpdate;

// Pins the OS behavior the whole self-update design depends on (ADR-001
// §1.1, plan §5.1): a running executable can be renamed aside and a new
// file put at its old path, and — for rollback — the backup can be moved
// back to that same path, all while the process keeps running from its
// original (now-renamed) image. If a future Windows change breaks this,
// this test fails instead of a user's install silently corrupting itself.
public class RunningProcessSwapFitnessTests {
    [Fact]
    [IntegrationTest]
    public void SwapAndRollback_AgainstALiveLockedExecutable_BothSucceedWhileProcessKeepsRunning() {
        using var directory = new TempDirectory(nameof(RunningProcessSwapFitnessTests));

        var systemPing = SysPath.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.System), "PING.EXE"
        );
        var runningExePath = directory.Combine("app.exe");
        var newExeContent = "new-build-stand-in"u8.ToArray();

        SysFile.Copy(systemPing, runningExePath);

        using var process = Process.Start(new ProcessStartInfo(runningExePath, "-t 127.0.0.1") {
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true
        })!;

        try {
            // Give the process a moment to actually open/lock its own
            // image before the swap begins.
            Thread.Sleep(200);
            Assert.False(process.HasExited);

            // --- swap: rename the running image aside, put a new file at its old path ---
            var backupPath = $"{runningExePath}.old-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}";

            SysFile.Move(runningExePath, backupPath);
            Assert.False(process.HasExited); // renaming the open image did not kill it

            SysFile.WriteAllBytes(runningExePath, newExeContent);
            Assert.True(SysFile.Exists(runningExePath));
            Assert.False(process.HasExited);

            // --- forced rollback: undo the swap while the process is still running ---
            SysFile.Delete(runningExePath);
            SysFile.Move(backupPath, runningExePath);

            Assert.False(process.HasExited);
            Assert.False(SysFile.Exists(backupPath));

            // The rolled-back file is once again the original running
            // image (same length as the untouched system PING.EXE), proving
            // the rollback moved the real backup back rather than leaving
            // the stand-in new content in place.
            Assert.Equal(new FileInfo(systemPing).Length, new FileInfo(runningExePath).Length);
        }
        finally {
            try { process.Kill(entireProcessTree: true); }
            catch (InvalidOperationException) { /* already exited */ }

            process.WaitForExit(TimeSpan.FromSeconds(5));
        }

        // Once the process has exited, the renamed-aside file (now rolled
        // back to its original path) can be deleted — the same cleanup
        // CleanupPreviousSystemUpdateStep performs on the next launch.
        SysFile.Delete(runningExePath);
    }
}

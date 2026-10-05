using System.Diagnostics;
using Nameless.Testing.Tools.IO;
using Xunit;

namespace Nameless.Testing.Tools.Attributes;

/// <summary>
///     A <see cref="FactAttribute"/> that is skipped when the current user
///     cannot create symbolic links (on Windows that requires Developer
///     Mode or elevation).
/// </summary>
public sealed class SymlinkFactAttribute : FactAttribute {
    private static readonly Lazy<bool> CanCreateSymlinks = new(Probe);

    public SymlinkFactAttribute() {
        if (!CanCreateSymlinks.Value) {
            Skip = "Symbolic links cannot be created by the current user.";
        }
    }

    private static bool Probe() {
        using var temp = new TempDirectory();
        var target = temp.CreateFile("target.txt", "x");

        try {
            SysFile.CreateSymbolicLink(temp.Combine("link.txt"), target);

            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) {
            return false;
        }
    }
}

public static class Links {
    /// <summary>
    ///     Creates an NTFS junction (no special privilege needed); Windows only.
    /// </summary>
    public static void CreateJunction(string path, string target) {
        using var process = Process.Start(new ProcessStartInfo("cmd.exe", $"/c mklink /J \"{path}\" \"{target}\"") {
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        })!;

        process.WaitForExit();

        if (process.ExitCode != 0) {
            throw new InvalidOperationException($"mklink /J failed: {process.StandardError.ReadToEnd()}");
        }
    }
}

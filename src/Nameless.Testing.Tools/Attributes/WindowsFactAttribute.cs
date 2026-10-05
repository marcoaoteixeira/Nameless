using Xunit;

namespace Nameless.Testing.Tools.Attributes;

/// <summary>
///     A <see cref="FactAttribute"/> that only runs on Windows
///     (drive letters, UNC roots).
/// </summary>
public sealed class WindowsFactAttribute : FactAttribute {
    public WindowsFactAttribute() {
        if (!OperatingSystem.IsWindows()) {
            Skip = "Windows-only path semantics.";
        }
    }
}

/// <summary>
///     A <see cref="TheoryAttribute"/> that only runs on Windows
///     (drive letters, UNC roots).
/// </summary>
public sealed class WindowsTheoryAttribute : TheoryAttribute {
    public WindowsTheoryAttribute() {
        if (!OperatingSystem.IsWindows()) {
            Skip = "Windows-only path semantics.";
        }
    }
}

/// <summary>
///     A <see cref="FactAttribute"/> that only runs outside Windows
///     (names Windows refuses to create).
/// </summary>
public sealed class NonWindowsFactAttribute : FactAttribute {
    public NonWindowsFactAttribute() {
        if (OperatingSystem.IsWindows()) {
            Skip = "Requires file names Windows does not allow.";
        }
    }
}

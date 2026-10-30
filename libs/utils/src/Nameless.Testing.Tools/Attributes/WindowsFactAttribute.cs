using Xunit;

namespace Nameless.Testing.Tools.Attributes;

/// <summary>
///     A <see cref="FactAttribute"/> that only runs on Windows
///     (drive letters, UNC roots).
/// </summary>
public sealed class WindowsFactAttribute : FactAttribute {
    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="WindowsFactAttribute"/> class.
    /// </summary>
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
    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="WindowsTheoryAttribute"/> class.
    /// </summary>
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
    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="NonWindowsFactAttribute"/> class.
    /// </summary>
    public NonWindowsFactAttribute() {
        if (OperatingSystem.IsWindows()) {
            Skip = "Requires file names Windows does not allow.";
        }
    }
}

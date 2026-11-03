using Nameless.WinApp.Infrastructure;

namespace Nameless.WinApp.Tests.Tooling;

// Test seam standing in for the real process facts ISystemUpdateEnvironment
// exposes — lets tests point "the install directory" and "the running exe"
// at a temp directory instead of the actual app's install location.
internal sealed class FakeSystemUpdateEnvironment : ISystemUpdateEnvironment {
    public required string InstallDirectoryPath { get; init; }
    public required string ExecutablePath { get; init; }
    public bool IsSelfUpdateEnabled { get; init; } = true;
}

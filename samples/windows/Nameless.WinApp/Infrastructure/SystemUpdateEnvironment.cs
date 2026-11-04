using System.Reflection;
using Nameless.IO;

namespace Nameless.WinApp.Infrastructure;

/// <summary>
///     Default <see cref="ISystemUpdateEnvironment"/> implementation.
/// </summary>
public sealed class SystemUpdateEnvironment : ISystemUpdateEnvironment {
    /// <inheritdoc />
    public string InstallDirectoryPath { get; } = PathHelper.RemoveTrailingSlash(PathHelper.Normalize(AppContext.BaseDirectory));

    /// <inheritdoc />
    public string ExecutablePath { get; } = Environment.ProcessPath ??
                                            throw new InvalidOperationException("The current process path could not be determined.");

    /// <inheritdoc />
    public bool IsSelfUpdateEnabled { get; } = Assembly.GetEntryAssembly()?
                                                       .GetCustomAttributes<AssemblyMetadataAttribute>()
                                                       .Any(attribute => attribute is { Key: "Nameless.WinApp.SelfUpdate", Value: "Enabled" }) ?? false;
}

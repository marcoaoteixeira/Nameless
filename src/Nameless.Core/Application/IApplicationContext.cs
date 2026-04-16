using Nameless.IO.FileSystem;

namespace Nameless.Application;

/// <summary>
///     Application Context Contract
/// </summary>
public interface IApplicationContext {
    /// <summary>
    ///     Gets the application environment name.
    /// </summary>
    string EnvironmentName { get; }

    /// <summary>
    ///     Gets the application name.
    /// </summary>
    string ApplicationName { get; }

    /// <summary>
    ///     Gets the path to the application directory where all
    ///     application files reside.
    /// </summary>
    string BaseDirectoryPath { get; }

    /// <summary>
    ///     Gets a <see cref="IFileSystemProvider"/> instance that is
    ///     bounded to the application data directory, which contains
    ///     the application data files.
    /// </summary>
    IFileSystemProvider FileSystemProvider { get; }

    /// <summary>
    ///     Gets the application version.
    /// </summary>
    string Version { get; }
}
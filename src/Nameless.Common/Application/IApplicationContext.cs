using Nameless.IO;

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
    ///     Gets the path to the application data directory.
    /// </summary>
    string ApplicationDataDirectory { get; }

    /// <summary>
    ///     Gets a <see cref="IFileProvider"/> instance that is
    ///     bounded to the application data directory.
    /// </summary>
    IFileProvider ApplicationDataFileProvider { get; }

    /// <summary>
    ///     Gets the application version.
    /// </summary>
    string Version { get; }

    /// <summary>
    ///     Retrieves the environment variable associated with the specified
    ///     key.
    /// </summary>
    /// <param name="key">
    ///     The environment variable key.
    /// </param>
    /// <returns>
    ///     If the environment variable is found, retrieves it; otherwise
    ///     returns <see langword="null"/>.
    /// </returns>
    string? GetEnvironmentVariable(string key);
}
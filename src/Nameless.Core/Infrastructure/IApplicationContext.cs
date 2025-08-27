namespace Nameless.Infrastructure;

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
    ///     Gets the path to the application files and folders directory,
    ///     which is the directory where the application is running from.
    /// </summary>
    string BaseDirectoryPath { get; }

    /// <summary>
    ///     Gets the path to the application data directory,
    ///     which is used to store application data files.
    /// </summary>
    string DataDirectoryPath { get; }

    /// <summary>
    ///     Gets the application version.
    /// </summary>
    string Version { get; }
}
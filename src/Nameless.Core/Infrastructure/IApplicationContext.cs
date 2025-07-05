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
    ///     Gets the path to the application folder.
    /// </summary>
    string ApplicationFolderPath { get; }

    /// <summary>
    ///     Gets the path to the application data folder.
    /// </summary>
    string ApplicationDataFolderPath { get; }

    /// <summary>
    ///     Gets the application version.
    /// </summary>
    string Version { get; }
}
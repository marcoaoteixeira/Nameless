namespace Nameless.Infrastructure;

/// <summary>
/// Application Context Contract
/// </summary>
public interface IApplicationContext {
    /// <summary>
    /// Gets the application environment name.
    /// </summary>
    string Environment { get; }

    /// <summary>
    /// Gets the application name.
    /// </summary>
    string AppName { get; }

    /// <summary>
    /// Gets the path to the application folder.
    /// </summary>
    string AppFolderPath { get; }

    /// <summary>
    /// Gets the path to the application data folder.
    /// </summary>
    string AppDataFolderPath { get; }

    /// <summary>
    /// Gets the application version.
    /// </summary>
    string Version { get; }
}
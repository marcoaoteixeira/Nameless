namespace Nameless.Infrastructure;

public interface IApplicationContext {
    /// <summary>
    /// Gets the current environment name.
    /// </summary>
    string EnvironmentName { get; }

    /// <summary>
    /// Gets the application name.
    /// </summary>
    string ApplicationName { get; }

    /// <summary>
    /// Gets the (full) path to the application folder.
    /// </summary>
    string ApplicationBasePath { get; }

    /// <summary>
    /// Gets the (full) path to the application data folder.
    /// </summary>
    string ApplicationDataFolderPath { get; }

    /// <summary>
    /// Gets the semantic version for the application.
    /// </summary>
    string SemVer { get; }
}
namespace Nameless.Infrastructure;

public interface IApplicationContext {
    /// <summary>
    /// Gets the current environment name. (e.g. Development, Production etc)
    /// </summary>
    string Environment { get; }

    /// <summary>
    /// Gets the application name.
    /// </summary>
    string AppName { get; }

    /// <summary>
    /// Gets the (full) path to the application folder.
    /// </summary>
    string AppBasePath { get; }

    /// <summary>
    /// Gets the (full) path to the application data folder.
    /// </summary>
    string AppDataFolderPath { get; }

    /// <summary>
    /// Gets the semantic version for the application.
    /// </summary>
    string SemVer { get; }
}
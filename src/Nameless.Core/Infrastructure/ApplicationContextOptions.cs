namespace Nameless.Infrastructure;

/// <summary>
///     Application context options.
/// </summary>
public sealed class ApplicationContextOptions {
    /// <summary>
    ///     Gets or sets the name of the current environment.
    /// </summary>
    public string EnvironmentName { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the name of the application.
    /// </summary>
    public string ApplicationName { get; set; } = AppDomain.CurrentDomain.FriendlyName;

    /// <summary>
    ///     Gets or sets where the application should store its data.
    /// </summary>
    public ApplicationDataLocation ApplicationDataLocation { get; set; }

    /// <summary>
    ///     Gets or sets the custom application data directory.
    /// </summary>
    public string? CustomApplicationDataDirectoryPath { get; set; }

    /// <summary>
    ///     Gets or sets the version of the application.
    /// </summary>
    public Version Version { get; set; } = new(1, 0, 0);
}
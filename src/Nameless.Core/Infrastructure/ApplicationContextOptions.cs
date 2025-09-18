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

/// <summary>
///     Defines where the application data directory will be located.
/// </summary>
public enum ApplicationDataLocation {
    /// <summary>
    ///     Directory location that is common to all users in the current
    ///     machine.
    /// </summary>
    /// <remarks>
    ///     <list type="bullet">
    ///         <item>
    ///             <term>On Windows</term>
    ///             <description>C:\ProgramData\APPLICATION_NAME</description>
    ///         </item>
    ///         <item>
    ///             <term>On Linux</term>
    ///             <description>/usr/share/APPLICATION_NAME</description>
    ///         </item>
    ///     </list>
    /// </remarks>
    Machine,

    /// <summary>
    ///     Directory location that is specific to the current user,
    ///     means non-roaming user.
    /// </summary>
    /// <remarks>
    ///     <list type="bullet">
    ///         <item>
    ///             <term>On Windows</term>
    ///             <description>C:\Users\CURRENT_USER\AppData\Local\APPLICATION_NAME</description>
    ///         </item>
    ///         <item>
    ///             <term>On Linux</term>
    ///             <description>/home/CURRENT_USER/.local/share/APPLICATION_NAME</description>
    ///         </item>
    ///     </list>
    /// </remarks>
    User,

    /// <summary>
    ///     When using <see cref="Custom"/>, it's necessary to provide a valid
    ///     directory path using the property
    ///     <see cref="ApplicationContextOptions.CustomApplicationDataDirectoryPath"/>.
    /// </summary>
    Custom
}
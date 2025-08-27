namespace Nameless.Infrastructure;

/// <summary>
///     Application context options.
/// </summary>
public sealed record ApplicationContextOptions {
    /// <summary>
    ///     Gets or sets the name of the current environment.
    /// </summary>
    public string EnvironmentName { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the name of the application.
    /// </summary>
    public string ApplicationName { get; set; } = AppDomain.CurrentDomain.FriendlyName;

    /// <summary>
    ///     Whether the application should use the
    ///     local application data directory.
    /// </summary>
    /// <remarks>
    ///     When <see cref="UseLocalApplicationData"/> is <see langword="true"/>, then
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
    ///     Otherwise,
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
    ///     Also, it will try to create the application data directory if it does
    ///     not exist. On failure, the exception will be thrown.
    /// </remarks>
    public bool UseLocalApplicationData { get; set; }

    /// <summary>
    ///     Gets or sets the version of the application.
    /// </summary>
    public Version Version { get; set; } = new(1, 0, 0);
}

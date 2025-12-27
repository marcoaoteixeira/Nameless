namespace Nameless.Infrastructure;

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
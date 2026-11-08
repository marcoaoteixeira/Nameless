namespace Nameless.Application;

/// <summary>
///     Defines where the application data directory will be located.
/// </summary>
public enum ApplicationDataLocation {
    /// <summary>
    ///     SystemDirectory location that is common to all users in the current
    ///     machine.
    /// </summary>
    /// <remarks>
    ///     <list type="bullet">
    ///         <item>
    ///             <term>On Windows</term>
    ///             <description>%ProgramData%\<c>Application_Name</c></description>
    ///         </item>
    ///         <item>
    ///             <term>On Linux</term>
    ///             <description>/usr/share/<c>Application_Name</c></description>
    ///         </item>
    ///     </list>
    /// </remarks>
    Machine,

    /// <summary>
    ///     SystemDirectory location that is specific to the current user,
    ///     means non-roaming user.
    /// </summary>
    /// <remarks>
    ///     <list type="bullet">
    ///         <item>
    ///             <term>On Windows</term>
    ///             <description>%LOCALAPPDATA%\<c>Application_Name</c></description>
    ///         </item>
    ///         <item>
    ///             <term>On Linux</term>
    ///             <description>~/.local/share/<c>Application_Name</c></description>
    ///         </item>
    ///     </list>
    /// </remarks>
    User,

    /// <summary>
    ///     Location relative to the application base directory.
    /// </summary>
    /// <remarks>
    ///     <list type="bullet">
    ///         <item>
    ///             <term>On Windows and Linux</term>
    ///             <description><c>AppContext.BaseDirectory</c>/App_Data</description>
    ///         </item>
    ///     </list>
    /// </remarks>
    App
}
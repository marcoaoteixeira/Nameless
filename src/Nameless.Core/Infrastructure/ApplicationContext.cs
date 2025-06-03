using Microsoft.Extensions.Logging;

namespace Nameless.Infrastructure;

public sealed class ApplicationContext : IApplicationContext {
    /// <summary>
    ///     Initializes a new instance of <see cref="ApplicationContext" />
    /// </summary>
    /// <param name="environment">The environment.</param>
    /// <param name="appName">The application name.</param>
    /// <param name="useCommonAppDataFolder">
    ///     Whether it will use the OS common data folder or the user level data folder.
    /// </param>
    /// <param name="appVersion">The application version.</param>
    /// <param name="logger">The logger.</param>
    /// <remarks>
    ///     When using <paramref name="useCommonAppDataFolder" /> if <c>true</c>, then
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
    ///     Otherwise,
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
    ///     Also, it will try to create the application data folder if it does not exist.
    ///     On failure, the error will be logged and the <see cref="AppDataFolderPath" /> will be empty.
    /// </remarks>
    public ApplicationContext(string environment, string appName, bool useCommonAppDataFolder, Version appVersion,
                              ILogger<ApplicationContext> logger) {
        Environment = Prevent.Argument.NullOrWhiteSpace(environment);
        AppName = Prevent.Argument.NullOrWhiteSpace(appName);
        AppFolderPath = AppDomain.CurrentDomain.BaseDirectory;
        AppDataFolderPath = BuildAppDataFolderPath(AppName, useCommonAppDataFolder, logger);

        Prevent.Argument.Null(appVersion);
        Version = $"v{appVersion.Major}.{appVersion.Minor}.{appVersion.Build}";
    }

    /// <inheritdoc />
    public string Environment { get; }

    /// <inheritdoc />
    public string AppName { get; }

    /// <inheritdoc />
    public string AppFolderPath { get; }

    /// <inheritdoc />
    public string AppDataFolderPath { get; }

    /// <inheritdoc />
    /// <remarks>The semantic version.</remarks>
    public string Version { get; }

    private static string BuildAppDataFolderPath(string appName, bool useCommonAppDataFolder,
                                                 ILogger<ApplicationContext> logger) {
        var specialFolder = useCommonAppDataFolder
            ? System.Environment.SpecialFolder.CommonApplicationData
            : System.Environment.SpecialFolder.LocalApplicationData;

        var specialFolderPath = System.Environment.GetFolderPath(specialFolder);
        var result = Path.Combine(specialFolderPath, appName);

        // Ensure directory exists
        try { Directory.CreateDirectory(result); }
        catch (Exception ex) {
            logger.ErrorOnAppDataFolderCreation(ex);
            return string.Empty;
        }

        return result;
    }
}
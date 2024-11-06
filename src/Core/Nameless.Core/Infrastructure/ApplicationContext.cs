using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
/* Unmerged change from project 'Nameless.Core (net8.0)'
Before:
using Nameless.Internals;
After:
using Nameless.Infrastructure;
using Nameless.Infrastructure;
using Nameless.Infrastructure.Impl;
using Nameless.Internals;
*/
using Nameless.Internals;

namespace Nameless.Infrastructure;

public sealed class ApplicationContext : IApplicationContext {
    /// <inheritdoc />
    public string Environment { get; }

    /// <inheritdoc />
    public string AppName { get; }

    /// <inheritdoc />
    public string AppBasePath { get; }

    /// <inheritdoc />
    public string AppDataFolderPath { get; }

    /// <inheritdoc />
    public string SemVer { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="ApplicationContext"/>
    /// </summary>
    /// <param name="hostEnvironment">The host environment.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="useSpecialFolder">
    /// If <c>true</c>, <see cref="AppDataFolderPath"/> will point to:
    /// <list type="bullet">
    ///     <item>
    ///         <term>Windows</term>
    ///         <description>C:\Users\CURRENT_USER\AppData\Local\APPLICATION_NAME</description>
    ///     </item>
    ///     <item>
    ///         <term>Linux</term>
    ///         <description>/CURRENT_USER/.local/share/APPLICATION_NAME</description>
    ///     </item>
    /// </list>
    /// Otherwise, will point to <see cref="AppBasePath"/> + "App_Data"
    /// </param>
    /// <param name="appVersion">The application version.</param>
    public ApplicationContext(IHostEnvironment hostEnvironment, ILogger<ApplicationContext> logger, bool useSpecialFolder = true, Version? appVersion = null) {
        Prevent.Argument.Null(hostEnvironment);
        Prevent.Argument.Null(logger);

        Environment = hostEnvironment.EnvironmentName;
        AppName = hostEnvironment.ApplicationName;
        AppBasePath = AppDomain.CurrentDomain.BaseDirectory;
        AppDataFolderPath = BuildDataFolderPath(AppName, AppBasePath, useSpecialFolder, logger);

        var version = appVersion ?? new Version(major: 0, minor: 0, build: 0);
        SemVer = $"v{version.Major}.{version.Minor}.{version.Build}";
    }

    private static string BuildDataFolderPath(string applicationName, string basePath, bool useSpecialFolder, ILogger<ApplicationContext> logger) {
        var specialFolder = System.Environment
                                  .GetFolderPath(folder: System.Environment
                                                               .SpecialFolder
                                                               .LocalApplicationData);

        var result = useSpecialFolder
            ? Path.Combine(specialFolder, applicationName)
            : Path.Combine(basePath, "App_Data");

        // Ensure directory exists
        try { Directory.CreateDirectory(result); } catch (Exception ex) { logger.ErrorOnDataFolderCreation(ex); }

        return result;
    }
}
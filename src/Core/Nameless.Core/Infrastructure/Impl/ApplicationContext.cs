using Microsoft.Extensions.Hosting;

namespace Nameless.Infrastructure.Impl;

public sealed class ApplicationContext : IApplicationContext {
    /// <summary>
    /// Gets the environment name. (e.g. Development, Production etc)
    /// See <see cref="IHostEnvironment.EnvironmentName"/>
    /// </summary>
    public string EnvironmentName { get; }

    /// <summary>
    /// Gets the application name.
    /// See <see cref="IHostEnvironment.ApplicationName"/>.
    /// </summary>
    public string ApplicationName { get; }

    /// <summary>
    /// Gets the base path of the application. This will always be
    /// the current location of the application assembly.
    /// </summary>
    public string ApplicationBasePath { get; }

    /// <summary>
    /// Gets the application data folder.
    /// </summary>
    public string ApplicationDataFolderPath { get; }

    /// <summary>
    /// Gets the semantic version of the application.
    /// </summary>
    public string SemVer { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="ApplicationContext"/>
    /// </summary>
    /// <param name="hostEnvironment">The host environment.</param>
    /// <param name="useSpecialFolder">
    /// If <c>true</c>, <see cref="ApplicationDataFolderPath"/> will point to:
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
    /// Otherwise, will point to <see cref="ApplicationBasePath"/> + "App_Data"
    /// </param>
    /// <param name="applicationVersion">The application version.</param>
    public ApplicationContext(IHostEnvironment hostEnvironment, bool useSpecialFolder = true, Version? applicationVersion = null) {
        Prevent.Argument.Null(hostEnvironment, nameof(hostEnvironment));

        EnvironmentName = hostEnvironment.EnvironmentName;
        ApplicationName = hostEnvironment.ApplicationName;
        ApplicationBasePath = AppDomain.CurrentDomain.BaseDirectory;

        ApplicationDataFolderPath = BuildApplicationDataFolderPath(ApplicationName, ApplicationBasePath, useSpecialFolder);

        var version = applicationVersion ?? new Version(major: 0, minor: 0, build: 0);
        SemVer = $"v{version.Major}.{version.Minor}.{version.Build}";
    }

    private static string BuildApplicationDataFolderPath(string applicationName, string basePath, bool useSpecialFolder) {
        var specialFolder = Environment.GetFolderPath(
            folder: Environment.SpecialFolder.LocalApplicationData
        );

        var result = useSpecialFolder
            ? Path.Combine(specialFolder, applicationName)
            : Path.Combine(basePath, "App_Data");

        // Ensure directory exists
        try { Directory.CreateDirectory(result); } catch (Exception ex) { Console.WriteLine(ex); }

        return result;
    }
}
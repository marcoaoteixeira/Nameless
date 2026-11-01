using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.IO;
using Nameless.IO.Physical;
using Nameless.ObjectModel;

namespace Nameless.Application;

/// <summary>
///     The application context.
/// </summary>
internal class ApplicationContext : IApplicationContext {
    private readonly IOptions<ApplicationContextOptions> _options;
    private readonly ILogger<ApplicationContext> _logger;

    /// <inheritdoc />
    public string EnvironmentName => _options.Value.EnvironmentName;

    /// <inheritdoc />
    public string ApplicationName => _options.Value.ApplicationName;
    
    /// <inheritdoc />
    public IFileManager FileManager { get; }

    /// <inheritdoc />
    public string Version { get; }

    /// <summary>
    ///     Initializes a new instance of <see cref="ApplicationContext" />
    /// </summary>
    /// <param name="options">The application context options.</param>
    /// <param name="logger">The logger.</param>
    // NOTE: ApplicationContext should never inject anything beside options
    // or logger. We want to guarantee that we can use the object during
    // other objects resolution.
    public ApplicationContext(IOptions<ApplicationContextOptions> options, ILogger<ApplicationContext> logger) {
        _options = options;
        _logger = logger;

        FileManager = CreateApplicationDataFileExplorer();
        Version = GetVersion();
    }

    /// <inheritdoc />
    public string? GetEnvironmentVariable(string key) {
        return Environment.GetEnvironmentVariable(key);
    }

    private string GetApplicationDataDirectory() {
        var appName = PathHelper.Sanitize(ApplicationName);

        var applicationDataDirectory = _options.Value.ApplicationDataLocation switch {
            ApplicationDataLocation.Machine => SysPath.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.CommonApplicationData
            ), appName),

            ApplicationDataLocation.User => SysPath.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData
            ), appName),

            _ => SysPath.Combine(AppContext.BaseDirectory, "App_Data")
        };

        return PathHelper.Normalize(
            SysDirectory.CreateDirectory(applicationDataDirectory).FullName
        );
    }

    private PhysicalFileManager CreateApplicationDataFileExplorer() {
        var directoryPath = GetApplicationDataDirectory();

        try { return new PhysicalFileManager(directoryPath); }
        catch (Exception ex) {
            CommonLog.Error(_logger, ex.Message, ex, GetType().Tag);

            throw;
        }
    }

    private string GetVersion() {
        var version = SemVersion.TryParse(_options.Value.Version, out var output)
            ? output
            : SemVersion.V1;

        return version.Format(includePrefix: true);
    }
}
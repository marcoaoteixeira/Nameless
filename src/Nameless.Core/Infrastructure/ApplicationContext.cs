using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.Helpers;
using Nameless.Internals;

namespace Nameless.Infrastructure;

public sealed class ApplicationContext : IApplicationContext {
    private readonly IOptions<ApplicationContextOptions> _options;
    private readonly ILogger<ApplicationContext> _logger;
    private readonly Lazy<string> _dataDirectoryPath;
    private readonly Lazy<string> _version;

    /// <summary>
    ///     Initializes a new instance of <see cref="ApplicationContext" />
    /// </summary>
    /// <param name="options">The application context options.</param>
    /// <param name="logger">The logger.</param>
    public ApplicationContext(IOptions<ApplicationContextOptions> options, ILogger<ApplicationContext> logger) {
        _options = options;
        _logger = logger;

        _dataDirectoryPath = new Lazy<string>(GetDataDirectoryPath);
        _version = new Lazy<string>(GetVersion);
    }

    /// <inheritdoc />
    public string EnvironmentName => _options.Value.EnvironmentName;

    /// <inheritdoc />
    public string ApplicationName => _options.Value.ApplicationName;

    /// <inheritdoc />
    public string BaseDirectoryPath => AppDomain.CurrentDomain.BaseDirectory;

    /// <inheritdoc />
    public string DataDirectoryPath => _dataDirectoryPath.Value;

    /// <inheritdoc />
    /// <remarks>The semantic version.</remarks>
    public string Version => _version.Value;

    private string GetDataDirectoryPath() {
        var options = _options.Value;

        var directoryPath = options.ApplicationDataLocation switch {
            ApplicationDataLocation.Machine => Environment.GetFolderPath(
                Environment.SpecialFolder.CommonApplicationData),
            ApplicationDataLocation.User => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            _ => options.CustomApplicationDataDirectoryPath
        };

        if (string.IsNullOrWhiteSpace(directoryPath)) {
            throw new InvalidOperationException(message: "Must provide application data directory path.");
        }

        directoryPath = PathHelper.Normalize(directoryPath);

        directoryPath = Path.IsPathRooted(directoryPath)
            ? Path.GetFullPath(directoryPath)
            : Path.GetFullPath(directoryPath, BaseDirectoryPath);

        if (options.ApplicationDataLocation != ApplicationDataLocation.Custom) {
            directoryPath = Path.Combine(directoryPath, ApplicationName);
        }

        // Ensure directory existence
        try { return Directory.CreateDirectory(directoryPath).FullName; }
        catch (Exception ex) {
            _logger.CreateDataDirectoryPathFailure(ex);

            throw;
        }
    }

    private string GetVersion() {
        var version = _options.Value.Version;

        return $"v{version.Major}.{version.Minor}.{version.Build}";
    }
}
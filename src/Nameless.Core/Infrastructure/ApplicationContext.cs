using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        _options = Guard.Against.Null(options);
        _logger = Guard.Against.Null(logger);

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

        var specialFolder = options.UseLocalApplicationData
            ? Environment.SpecialFolder.LocalApplicationData
            : Environment.SpecialFolder.CommonApplicationData;

        var specialDirectoryPath = Environment.GetFolderPath(specialFolder);
        var result = Path.Combine(specialDirectoryPath, options.ApplicationName);

        try {
            // Ensure directory exists
            return Directory.CreateDirectory(result).FullName;
        }
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

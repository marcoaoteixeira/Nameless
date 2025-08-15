using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.Internals;

namespace Nameless.Infrastructure;

public sealed class ApplicationContext : IApplicationContext {
    private readonly IOptions<ApplicationContextOptions> _options;
    private readonly ILogger<ApplicationContext> _logger;
    private readonly Lazy<string> _applicationDataFolderPath;
    private readonly Lazy<string> _version;

    /// <summary>
    ///     Initializes a new instance of <see cref="ApplicationContext" />
    /// </summary>
    /// <param name="options">The application context options.</param>
    /// <param name="logger">The logger.</param>
    public ApplicationContext(IOptions<ApplicationContextOptions> options, ILogger<ApplicationContext> logger) {
        _options = Guard.Against.Null(options);
        _logger = Guard.Against.Null(logger);

        _applicationDataFolderPath = new Lazy<string>(CreateApplicationDataFolderPath);
        _version = new Lazy<string>(CreateVersion);
    }

    /// <inheritdoc />
    public string EnvironmentName => _options.Value.EnvironmentName;

    /// <inheritdoc />
    public string ApplicationName => _options.Value.ApplicationName;

    /// <inheritdoc />
    public string ApplicationFolderPath => AppDomain.CurrentDomain.BaseDirectory;

    /// <inheritdoc />
    public string ApplicationDataFolderPath => _applicationDataFolderPath.Value;

    /// <inheritdoc />
    /// <remarks>The semantic version.</remarks>
    public string Version => _version.Value;

    private string CreateApplicationDataFolderPath() {
        var options = _options.Value;

        var specialFolder = options.UseCommonAppDataFolder
            ? Environment.SpecialFolder.CommonApplicationData
            : Environment.SpecialFolder.LocalApplicationData;

        var specialFolderPath = Environment.GetFolderPath(specialFolder);
        var result = Path.Combine(specialFolderPath, options.ApplicationName);

        // Ensure directory exists
        try { Directory.CreateDirectory(result); }
        catch (Exception ex) {
            _logger.ErrorOnAppDataFolderCreation(ex);
            return string.Empty;
        }

        return result;
    }

    private string CreateVersion() {
        var version = _options.Value.Version;

        return $"v{version.Major}.{version.Minor}.{version.Build}";
    }
}

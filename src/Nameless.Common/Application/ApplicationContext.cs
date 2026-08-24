using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.IO;
using Nameless.IO.Explorer;
using Nameless.ObjectModel;

namespace Nameless.Application;

/// <summary>
///     The application context.
/// </summary>
public class ApplicationContext : IApplicationContext {
    private const string LOG_TAG = "APPLICATION_CONTEXT";

    private readonly IOptions<ApplicationContextOptions> _options;
    private readonly ILogger<ApplicationContext> _logger;
    private readonly Lazy<IFileExplorer> _fileSystemProvider;

    /// <inheritdoc />
    public string EnvironmentName => _options.Value.EnvironmentName;

    /// <inheritdoc />
    public string ApplicationName => _options.Value.ApplicationName;

    /// <inheritdoc />
    public string BaseDirectoryPath => AppDomain.CurrentDomain.BaseDirectory;

    /// <inheritdoc />
    public IFileExplorer FileExplorer => _fileSystemProvider.Value;

    /// <inheritdoc />
    public string Version => GetVersion().Format(includePrefix: true);

    /// <summary>
    ///     Initializes a new instance of <see cref="ApplicationContext" />
    /// </summary>
    /// <param name="options">The application context options.</param>
    /// <param name="logger">The logger.</param>
    public ApplicationContext(IOptions<ApplicationContextOptions> options, ILogger<ApplicationContext> logger) {
        _options = options;
        _logger = logger;

        _fileSystemProvider = new Lazy<IFileExplorer>(CreateFileSystemProvider);
    }

    private FileExplorer CreateFileSystemProvider() {
        var options = _options.Value;
        var appName = PathHelper.Sanitize(ApplicationName);
        var directoryPath = options.ApplicationDataLocation switch {
            ApplicationDataLocation.Machine => Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.CommonApplicationData
            ), appName),
            
            ApplicationDataLocation.User => Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData
            ), appName),
            
            _ => Path.Combine(BaseDirectoryPath, "App_Data")
        };

        directoryPath = PathHelper.Normalize(directoryPath);

        try {
            // Ensure directory existence
            Directory.CreateDirectory(directoryPath);

            return new FileExplorer(
                options: Options.Create(new FileExplorerOptions {
                    AllowOperationOutsideRoot = false,
                    Root = directoryPath
                })
            );
        }
        catch (Exception ex) {
            CommonLog.Failure(_logger, ex, tag: LOG_TAG);

            throw;
        }
    }

    private SemVersion GetVersion() {
        return SemVersion.TryParse(_options.Value.Version, out var output)
            ? output
            : SemVersion.V1;
    }
}
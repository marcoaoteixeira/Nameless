using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.Helpers;
using Nameless.IO.FileSystem;

namespace Nameless.Application;

public class ApplicationContext : IApplicationContext {
    private readonly IOptions<ApplicationContextOptions> _options;
    private readonly ILogger<ApplicationContext> _logger;
    private readonly Lazy<IFileSystemProvider> _fileSystemProvider;

    /// <inheritdoc />
    public string EnvironmentName => _options.Value.EnvironmentName;

    /// <inheritdoc />
    public string ApplicationName => _options.Value.ApplicationName;

    /// <inheritdoc />
    public string BaseDirectoryPath => AppDomain.CurrentDomain.BaseDirectory;

    /// <inheritdoc />
    public IFileSystemProvider FileSystemProvider => _fileSystemProvider.Value;

    /// <inheritdoc />
    public string Version {
        get => field ??= _options.Value.Version.ToString(prefix: "v");
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="ApplicationContext" />
    /// </summary>
    /// <param name="options">The application context options.</param>
    /// <param name="logger">The logger.</param>
    public ApplicationContext(IOptions<ApplicationContextOptions> options, ILogger<ApplicationContext> logger) {
        _options = options;
        _logger = logger;

        _fileSystemProvider = new Lazy<IFileSystemProvider>(CreateFileSystemProvider);
    }

    private FileSystemProvider CreateFileSystemProvider() {
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

            var fspOptions = Options.Create(new FileSystemProviderOptions {
                AllowOperationOutsideRoot = false,
                Root = directoryPath
            });

            return new FileSystemProvider(fspOptions);
        }
        catch (Exception ex) {
            _logger.CreateFileSystemProviderFailure(ex);

            throw;
        }
    }
}
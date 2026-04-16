using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.Helpers;
using Nameless.IO.FileSystem;
using Nameless.IO.FileSystem.Impl;

namespace Nameless.Infrastructure;

public class ApplicationContext : IApplicationContext {
    private readonly IOptions<ApplicationContextOptions> _options;
    private readonly ILogger<ApplicationContext> _logger;
    private readonly Lazy<IFileSystem> _fileSystem;
    private readonly Lazy<string> _version;

    /// <inheritdoc />
    public string EnvironmentName => _options.Value.EnvironmentName;

    /// <inheritdoc />
    public string ApplicationName => _options.Value.ApplicationName;

    /// <inheritdoc />
    public string BaseDirectoryPath => AppDomain.CurrentDomain.BaseDirectory;

    /// <inheritdoc />
    public IFileSystem FileSystem => _fileSystem.Value;

    /// <inheritdoc />
    /// <remarks>The semantic version.</remarks>
    public string Version => _version.Value;

    /// <summary>
    ///     Initializes a new instance of <see cref="ApplicationContext" />
    /// </summary>
    /// <param name="options">The application context options.</param>
    /// <param name="logger">The logger.</param>
    public ApplicationContext(IOptions<ApplicationContextOptions> options, ILogger<ApplicationContext> logger) {
        _options = options;
        _logger = logger;

        _fileSystem = new Lazy<IFileSystem>(CreateFileSystemForDataDirectory);
        _version = new Lazy<string>(GetVersion);
    }

    private FileSystemImpl CreateFileSystemForDataDirectory() {
        var options = _options.Value;

        var directoryPath = options.ApplicationDataLocation switch {
            ApplicationDataLocation.Machine => Environment.GetFolderPath(
                Environment.SpecialFolder.CommonApplicationData
            ),
            
            ApplicationDataLocation.User => Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData
            ),
            
            _ => options.CustomApplicationDataDirectoryPath
        };

        if (string.IsNullOrWhiteSpace(directoryPath)) {
            throw new InvalidOperationException(
                "Must provide application data directory path."
            );
        }

        directoryPath = Path.IsPathRooted(directoryPath)
            ? Path.GetFullPath(directoryPath)
            : Path.GetFullPath(directoryPath, BaseDirectoryPath);

        if (options.ApplicationDataLocation != ApplicationDataLocation.Custom) {
            directoryPath = Path.Combine(directoryPath, ApplicationName);
        }

        directoryPath = PathHelper.Normalize(directoryPath);

        try {
            return new FileSystemImpl(Options.Create(new FileSystemOptions {
                AllowOperationOutsideRoot = false,
                Root = Directory.CreateDirectory(directoryPath).FullName // Ensure directory existence
            }));
        }
        catch (Exception ex) {
            _logger.CreateFileSystemForDataDirectoryFailure(ex);

            throw;
        }
    }

    private string GetVersion() {
        var version = _options.Value.Version;

        return $"v{version.Major}.{version.Minor}.{version.Build}";
    }
}
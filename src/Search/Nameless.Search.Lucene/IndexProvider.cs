using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.Helpers;
using Nameless.Infrastructure;
using Nameless.IO;
using Nameless.Search.Lucene.Options;

namespace Nameless.Search.Lucene;

/// <summary>
/// Default implementation of <see cref="IIndexProvider"/>
/// </summary>
public sealed class IndexProvider : IIndexProvider, IDisposable {
    private readonly IAnalyzerProvider _analyzerProvider;
    private readonly IApplicationContext _applicationContext;
    private readonly IFileSystem _fileSystem;
    private readonly IOptions<LuceneOptions> _options;

    private readonly ILogger<IndexProvider> _logger;
    private readonly ILogger<Index> _loggerForIndex;

    private ConcurrentDictionary<string, IIndex> Cache { get; } = new(StringComparer.OrdinalIgnoreCase);

    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of <see cref="IndexProvider"/>.
    /// </summary>
    /// <param name="analyzerProvider">The analyzer provider.</param>
    /// <param name="applicationContext">The application context.</param>
    /// <param name="fileSystem">The file system services.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="options">The settings.</param>
    public IndexProvider(IAnalyzerProvider analyzerProvider,
                         IApplicationContext applicationContext,
                         IFileSystem fileSystem,
                         ILoggerFactory loggerFactory,
                         IOptions<LuceneOptions> options) {
        Prevent.Argument.Null(loggerFactory);

        _applicationContext = Prevent.Argument.Null(applicationContext);
        _analyzerProvider = Prevent.Argument.Null(analyzerProvider);
        _fileSystem = Prevent.Argument.Null(fileSystem);

        _logger = loggerFactory.CreateLogger<IndexProvider>();
        _loggerForIndex = loggerFactory.CreateLogger<Index>();

        _options = Prevent.Argument.Null(options);
    }

    ~IndexProvider() {
        Dispose(disposing: false);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="name"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// if <paramref name="name"/> is empty or white spaces.
    /// </exception>
    public bool DeleteIndex(string name) {
        BlockAccessAfterDispose();

        Prevent.Argument.NullOrWhiteSpace(name);

        RemoveIndexFromCache(name);

        return DeleteIndexDirectory(name);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="name"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// if <paramref name="name"/> is empty or white spaces.
    /// </exception>
    public bool IndexExists(string name) {
        BlockAccessAfterDispose();

        Prevent.Argument.NullOrWhiteSpace(name);

        var indexDirectoryPath = GetIndexDirectoryPath(name);

        return _fileSystem.Directory.Exists(indexDirectoryPath);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="name"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// if <paramref name="name"/> is empty or white spaces.
    /// </exception>
    public IIndex CreateIndex(string name) {
        BlockAccessAfterDispose();

        Prevent.Argument.NullOrWhiteSpace(name);

        return Cache.GetOrAdd(name, Create);
    }

    /// <inheritdoc />
    public IEnumerable<string> ListIndexes() {
        BlockAccessAfterDispose();

        var rootPath = _fileSystem.Path.Combine(_applicationContext.AppDataFolderPath,
                                                _options.Value.LuceneFolderName);
        var directories = _fileSystem.Directory.GetDirectories(rootPath);

        foreach (var directory in directories) {
            yield return _fileSystem.Path.GetFileName(directory.Path);
        }
    }

    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private bool DeleteIndexDirectory(string indexName) {
        var indexDirectoryPath = GetIndexDirectoryPath(indexName);

        try {
            if (_fileSystem.Directory.Exists(indexDirectoryPath)) {
                _fileSystem.Directory.Delete(directoryPath: indexDirectoryPath,
                                             recursive: true);
            }
        } catch (Exception ex) {
            _logger.DeleteIndexDirectoryError(indexName, ex);

            return false;
        }

        return true;
    }

    private void RemoveIndexFromCache(string name) {
        if (Cache.TryRemove(name, out var index) &&
            index is IDisposable disposable) {
            disposable.Dispose();
        }
    }

    private void BlockAccessAfterDispose() {
#if NET8_0_OR_GREATER
#pragma warning disable IDE0022
        ObjectDisposedException.ThrowIf(_disposed, this);
#pragma warning restore IDE0022
#else
        if (_disposed) {
            throw new ObjectDisposedException(nameof(IndexProvider));
        }
#endif
    }

    private void Dispose(bool disposing) {
        if (_disposed) { return; }

        if (disposing) {
            var indexes = Cache.Values.ToArray();
            foreach (var index in indexes) {
                if (index is IDisposable disposable) {
                    disposable.Dispose();
                }
            }
            Cache.Clear();
        }

        _disposed = true;
    }

    private string GetIndexDirectoryPath(string indexName) {
        var path = _fileSystem.Path.Combine(_applicationContext.AppDataFolderPath,
                                            _options.Value.LuceneFolderName,
                                            indexName);

        return PathHelper.Normalize(path);
    }

    private Index Create(string indexName) {
        var indexDirectoryPath = GetIndexDirectoryPath(indexName);
        var directory = _fileSystem.Directory.Create(indexDirectoryPath);

        return new Index(analyzer: _analyzerProvider.GetAnalyzer(indexName),
                         indexDirectoryPath: directory.Path,
                         indexName: indexName,
                         logger: _loggerForIndex);
    }
}
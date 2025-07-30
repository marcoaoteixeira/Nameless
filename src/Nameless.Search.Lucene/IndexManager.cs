using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.Helpers;
using Nameless.Infrastructure;
using Nameless.Search.Lucene.Internals;

namespace Nameless.Search.Lucene;

/// <summary>
///     Default implementation of <see cref="IIndexManager" />
/// </summary>
public sealed class IndexManager : IIndexManager, IDisposable {
    private readonly IAnalyzerProvider _analyzerProvider;
    private readonly ILogger<IndexManager> _logger;
    private readonly ILogger<Index> _loggerForIndex;
    private readonly string _rootPath;

    private bool _disposed;

    private ConcurrentDictionary<string, IIndex> Cache { get; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     Initializes a new instance of <see cref="IndexManager" />.
    /// </summary>
    /// <param name="analyzerProvider">The analyzer provider.</param>
    /// <param name="applicationContext">The application context.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="options">The settings.</param>
    public IndexManager(IAnalyzerProvider analyzerProvider,
        IApplicationContext applicationContext,
        ILoggerFactory loggerFactory,
        IOptions<SearchOptions> options) {
        Prevent.Argument.Null(applicationContext);
        Prevent.Argument.Null(loggerFactory);
        Prevent.Argument.Null(options);

        _analyzerProvider = Prevent.Argument.Null(analyzerProvider);
        _logger = loggerFactory.CreateLogger<IndexManager>();
        _loggerForIndex = loggerFactory.CreateLogger<Index>();

        var rootPath = Path.Combine(
            applicationContext.ApplicationDataFolderPath,
            options.Value.IndexesFolderName);

        _rootPath = PathHelper.Normalize(rootPath);
    }

    ~IndexManager() {
        Dispose(false);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="name" /> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     if <paramref name="name" /> is empty or white spaces.
    /// </exception>
    public bool DeleteIndex(string name) {
        BlockAccessAfterDispose();

        Prevent.Argument.NullOrWhiteSpace(name);

        RemoveIndexFromCache(name);

        return DeleteIndexDirectory(name);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="name" /> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     if <paramref name="name" /> is empty or white spaces.
    /// </exception>
    public bool IndexExists(string name) {
        BlockAccessAfterDispose();

        Prevent.Argument.NullOrWhiteSpace(name);

        var indexDirectoryPath = GetIndexDirectoryPath(name);

        return Directory.Exists(indexDirectoryPath);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="name" /> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     if <paramref name="name" /> is empty or white spaces.
    /// </exception>
    public IIndex CreateIndex(string name) {
        BlockAccessAfterDispose();

        Prevent.Argument.NullOrWhiteSpace(name);

        return Cache.GetOrAdd(name, Create);
    }

    /// <inheritdoc />
    public IEnumerable<string> ListIndexes() {
        BlockAccessAfterDispose();

        var directories = Directory.GetDirectories(_rootPath);

        foreach (var directory in directories) {
            yield return Path.GetFileName(directory);
        }
    }

    /// <inheritdoc />
    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private bool DeleteIndexDirectory(string indexName) {
        var indexDirectoryPath = GetIndexDirectoryPath(indexName);

        try {
            if (Directory.Exists(indexDirectoryPath)) {
                Directory.Delete(indexDirectoryPath, recursive: true);
            }
        }
        catch (Exception ex) {
            _logger.DeleteIndexDirectoryError(indexName, ex);

            return false;
        }

        return true;
    }

    private void RemoveIndexFromCache(string name) {
        if (Cache.TryRemove(name, out var index) && index is IDisposable disposable) {
            _logger.IndexRemovedFromCache(name);

            disposable.Dispose();
        }
    }

    private void BlockAccessAfterDispose() {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    private void Dispose(bool disposing) {
        if (_disposed) { return; }

        if (disposing) {
            var keys = Cache.Keys.ToArray();
            foreach (var key in keys) {
                RemoveIndexFromCache(key);
            }
        }

        _disposed = true;
    }

    private string GetIndexDirectoryPath(string indexName) {
        return Path.Combine(_rootPath, indexName);
    }

    private Index Create(string indexName) {
        var indexDirectoryPath = GetIndexDirectoryPath(indexName);
        var directory = Directory.CreateDirectory(indexDirectoryPath);
        var index = new Index(_analyzerProvider.GetAnalyzer(indexName), directory.FullName, indexName, _loggerForIndex);

        index.OnDispose += (_, args) => {
            RemoveIndexFromCache(args.IndexName);
        };

        return index;
    }
}
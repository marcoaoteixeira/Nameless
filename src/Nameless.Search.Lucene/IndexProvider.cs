using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.Helpers;
using Nameless.Infrastructure;

namespace Nameless.Search.Lucene;

/// <summary>
///     Default implementation of <see cref="IIndexProvider" />
/// </summary>
public sealed class IndexProvider : IIndexProvider, IDisposable {
    private readonly IAnalyzerProvider _analyzerProvider;
    private readonly ILogger<IndexProvider> _logger;
    private readonly ILogger<Index> _loggerForIndex;
    private readonly string _rootPath;

    private bool _disposed;

    private ConcurrentDictionary<string, IIndex> Cache { get; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     Initializes a new instance of <see cref="IndexProvider" />.
    /// </summary>
    /// <param name="analyzerProvider">The analyzer provider.</param>
    /// <param name="applicationContext">The application context.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="options">The settings.</param>
    public IndexProvider(IAnalyzerProvider analyzerProvider,
                         IApplicationContext applicationContext,
                         ILoggerFactory loggerFactory,
                         IOptions<LuceneOptions> options) {
        Prevent.Argument.Null(applicationContext);
        Prevent.Argument.Null(loggerFactory);
        Prevent.Argument.Null(options);

        _analyzerProvider = Prevent.Argument.Null(analyzerProvider);

        _logger = loggerFactory.CreateLogger<IndexProvider>();
        _loggerForIndex = loggerFactory.CreateLogger<Index>();

        var rootPath = Path.Combine(applicationContext.AppDataFolderPath,
            options.Value.LuceneFolderName);

        _rootPath = PathHelper.Normalize(rootPath);
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="name" /> is <c>null</c>.
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
    ///     if <paramref name="name" /> is <c>null</c>.
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
    ///     if <paramref name="name" /> is <c>null</c>.
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

    ~IndexProvider() {
        Dispose(false);
    }

    private bool DeleteIndexDirectory(string indexName) {
        var indexDirectoryPath = GetIndexDirectoryPath(indexName);

        try {
            if (Directory.Exists(indexDirectoryPath)) {
                Directory.Delete(indexDirectoryPath,
                    true);
            }
        }
        catch (Exception ex) {
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
        return Path.Combine(_rootPath, indexName);
    }

    private Index Create(string indexName) {
        var indexDirectoryPath = GetIndexDirectoryPath(indexName);
        var directory = Directory.CreateDirectory(indexDirectoryPath);

        return new Index(_analyzerProvider.GetAnalyzer(indexName),
            directory.FullName,
            indexName,
            _loggerForIndex);
    }
}
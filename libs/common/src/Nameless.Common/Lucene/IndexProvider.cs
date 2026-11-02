using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.Contracts;
using Nameless.IO;

namespace Nameless.Lucene;

/// <summary>
///     Default implementation of <see cref="IIndexProvider" />.
/// </summary>
public class IndexProvider : IIndexProvider {
    private readonly ConcurrentDictionary<string, Index> _cache = new(StringComparer.InvariantCulture);
    private readonly IAnalyzerProvider _analyzerProvider;
    private readonly IFileManager _fileManager;
    private readonly IOptions<LuceneOptions> _options;
    private readonly ILogger<Index> _logger;

    private bool _disposed;

    /// <summary>
    ///     Initializes a new instance of the <see cref="IndexProvider" />
    ///     class.
    /// </summary>
    /// <param name="analyzerProvider">
    ///     The analyzer provider.
    /// </param>
    /// <param name="fileManager">
    ///     The file system.
    /// </param>
    /// <param name="options">
    ///     The lucene options.
    /// </param>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    public IndexProvider(
        IAnalyzerProvider analyzerProvider,
        IFileManager fileManager,
        IOptions<LuceneOptions> options,
        ILogger<Index> logger) {
        _analyzerProvider = analyzerProvider;
        _fileManager = fileManager;
        _options = options;
        _logger = logger;
    }

    /// <summary>
    ///     Destructor
    /// </summary>
    ~IndexProvider() {
        Dispose(disposing: false);
    }

    /// <inheritdoc />
    public IIndex Get(string? indexName) {
        BlockAccessAfterDispose();

        indexName ??= LuceneDefaults.IndexName;

        Throws.When.NoMatchingPattern(indexName, RegexCache.IndexNamePattern());

        return _cache.GetOrAdd(indexName, Create);
    }

    /// <inheritdoc />
    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public void CloseAll() {
        BlockAccessAfterDispose();

        CloseIndexes(_cache.Values.ToArray());
    }

    private void BlockAccessAfterDispose() {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    private void Dispose(bool disposing) {
        if (_disposed) { return; }

        if (disposing) {
            CloseIndexes(_cache.Values.ToArray());
        }

        _disposed = true;
    }

    private static void CloseIndexes(Index[] indexes) {
        List<Exception>? exceptions = null;

        foreach (var index in indexes) {
            try { index.Dispose(); }
            catch (Exception ex) {
                exceptions ??= [];
                exceptions.Add(ex);
            }
        }

        if (exceptions is not null) {
            throw new AggregateException(exceptions);
        }
    }

    private Index Create(string indexName) {
        var analyzer = _analyzerProvider.GetAnalyzer(indexName);
        var index = new Index(analyzer, _fileManager, indexName, _options, _logger);

        index.RegisterDisposeCallback(RemoveFromCache);

        return index;
    }

    private void RemoveFromCache(Index index) {
        // Remove by key+value match, not key-only: if a new instance was
        // already cached under the same name (e.g. a concurrent Get() racing
        // this dispose callback), a key-only removal would evict that new
        // instance instead, orphaning it (never disposed, keeps write.lock).
        if (_cache.TryRemove(new KeyValuePair<string, Index>(index.Name, index))) {
            index.UnregisterDisposeCallback(RemoveFromCache);
        }
    }
}
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.IO.FileSystem;

namespace Nameless.Lucene;

/// <summary>
///     Default implementation of <see cref="IIndexProvider" />.
/// </summary>
public sealed class IndexProvider : IIndexProvider {
    private static readonly ConcurrentDictionary<string, Index> Cache = new();

    private readonly IAnalyzerProvider _analyzerProvider;
    private readonly IFileSystem _fileSystem;
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
    /// <param name="fileSystem">
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
        IFileSystem fileSystem,
        IOptions<LuceneOptions> options,
        ILogger<Index> logger) {
        _analyzerProvider = Guard.Against.Null(analyzerProvider);
        _fileSystem = Guard.Against.Null(fileSystem);
        _options = Guard.Against.Null(options);
        _logger = Guard.Against.Null(logger);
    }

    ~IndexProvider() {
        Dispose(disposing: false);
    }

    /// <inheritdoc />
    public IIndex Get(string indexName) {
        BlockAccessAfterDispose();

        return Cache.GetOrAdd(indexName, Create);
    }

    /// <inheritdoc />
    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void BlockAccessAfterDispose() {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    private void Dispose(bool disposing) {
        if (_disposed) { return; }

        if (disposing) {
            var repositories = Cache.Values.ToArray();

            foreach (var repository in repositories) {
                repository.Dispose();
            }
        }

        _disposed = true;
    }

    private Index Create(string indexName) {
        var analyzer = _analyzerProvider.GetAnalyzer(indexName);
        var index = new Index(analyzer, _fileSystem, indexName, _options, _logger);

        index.RegisterDisposeCallback(RemoveFromCache);

        return index;
    }

    private void RemoveFromCache(Index index) {
        Cache.TryRemove(index.Name, out _);
    }
}
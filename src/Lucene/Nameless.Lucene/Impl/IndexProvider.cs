using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Nameless.Infrastructure;
using Nameless.Lucene.Internals;
using Nameless.Lucene.Options;
using Polly;
using Polly.Retry;

namespace Nameless.Lucene.Impl;

/// <summary>
/// Default implementation of <see cref="IIndexProvider"/>
/// </summary>
public sealed class IndexProvider : IIndexProvider, IDisposable {
    private const int MAX_DELETE_INDEX_RETRY_ATTEMPTS = 3;
    
    private static readonly object SyncLock = new();

    private readonly IApplicationContext _applicationContext;
    private readonly IAnalyzerProvider _analyzerProvider;
    private readonly ILogger<Index> _logger;
    private readonly LuceneOptions _options;

    private readonly RetryPolicy _deleteIndexRetryPolicy;

    private ConcurrentDictionary<string, IIndex> Cache { get; } = new(StringComparer.InvariantCultureIgnoreCase);
    
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of <see cref="IndexProvider"/>.
    /// </summary>
    /// <param name="applicationContext">The application context.</param>
    /// <param name="analyzerProvider">The analyzer provider.</param>
    /// <param name="logger">The <see cref="ILogger{Index}"/> that will be passed to the Index when created.</param>
    /// <param name="options">The settings.</param>
    public IndexProvider(IApplicationContext applicationContext, IAnalyzerProvider analyzerProvider, ILogger<Index> logger, LuceneOptions options) {
        _applicationContext = Prevent.Argument.Null(applicationContext, nameof(applicationContext));
        _analyzerProvider = Prevent.Argument.Null(analyzerProvider, nameof(analyzerProvider));
        _logger = Prevent.Argument.Null(logger, nameof(logger));
        _options = Prevent.Argument.Null(options, nameof(options));

        _deleteIndexRetryPolicy = Policy.Handle<Exception>(exception => {
                                            LoggerHelper.DeleteIndexDirectory(_logger, exception);
                                            return false;
                                        })
                                        .WaitAndRetry(retryCount: MAX_DELETE_INDEX_RETRY_ATTEMPTS,
                                                      sleepDurationProvider: retry => TimeSpan.FromSeconds(retry));
    }

    ~IndexProvider() {
        Dispose(disposing: false);
    }

    /// <inheritdoc />
    public void DeleteIndex(string indexName) {
        BlockAccessAfterDispose();

        lock (SyncLock) {
            // ReSharper disable once InconsistentNaming
            const int MaxAttempts = 3;

            // Get index directory path
            var indexDirectoryPath = GetIndexDirectoryPath(indexName);

            // Check if directory still exists.
            if (!Directory.Exists(indexDirectoryPath)) { return; }

            _deleteIndexRetryPolicy.Execute(() => {
                Directory.Delete(path: indexDirectoryPath, recursive: true);
            });

            // Retry policy in case of failure
            var count = 0;
            do {
                try {
                    Directory.Delete(path: indexDirectoryPath, recursive: true);
                    break;
                } catch (Exception ex) {
                    LoggerHelper.DeleteIndexDirectory(_logger, ex);
                }
            } while (++count < MaxAttempts);    
        }
    }

    /// <inheritdoc />
    public bool IndexExists(string indexName) {
        BlockAccessAfterDispose();

        return Cache.ContainsKey(indexName);
    }

    /// <inheritdoc />
    public IIndex GetOrCreateIndex(string indexName) {
        BlockAccessAfterDispose();

        return Cache.GetOrAdd(indexName, Create);
    }

    /// <inheritdoc />
    public IEnumerable<string> ListIndexes() {
        BlockAccessAfterDispose();

        return Cache.Keys;
    }

    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void BlockAccessAfterDispose() {
        if (_disposed) {
            throw new ObjectDisposedException(nameof(IndexProvider));
        }
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
        var path = Path.Combine(_applicationContext.ApplicationDataFolderPath,
                                _options.IndexesFolderName,
                                indexName);

        return Directory.CreateDirectory(path).FullName;
    }

    private Index Create(string indexName)
        => new(analyzer: _analyzerProvider.GetAnalyzer(indexName),
               indexDirectoryPath: GetIndexDirectoryPath(indexName),
               logger: _logger,
               name: indexName);
}
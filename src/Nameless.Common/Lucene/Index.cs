#pragma warning disable CA1859
// ReSharper disable InconsistentNaming

using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.IO.Explorer;
using Nameless.Lucene.Collections;
using Nameless.Lucene.Empty;
using Nameless.Lucene.ObjectModel;
using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Lucene;

/// <summary>
///     Default implementation of <see cref="IIndex"/>.
/// </summary>
public class Index : IIndex {
    private string LogTag => $"INDEX::{Name}";

    private readonly Analyzer _analyzer;
    private readonly IFileExplorer _fileSystemProvider;
    private readonly IOptions<LuceneOptions> _options;
    private readonly ILogger<Index> _logger;

    private IndexWriter? _indexWriter;
    private IndexReader? _indexReader;
    private Lazy<IndexWriterConfig>? _indexWriterConfig;
    private Lazy<FSDirectory>? _fsDirectory;
    private Action<Index>? _disposeCallback;
    private bool _disposed;

    /// <inheritdoc />
    public string Name { get; }

    private FSDirectory FSDirectory => _fsDirectory?.Value ??
                                       throw new InvalidOperationException($"{nameof(FSDirectory)} not available.");

    private IndexWriterConfig IndexWriterConfig => _indexWriterConfig?.Value ??
                                                   throw new InvalidOperationException($"{nameof(IndexWriterConfig)} not available.");

    /// <summary>
    ///     Initializes a new instance of <see cref="Index"/>.
    /// </summary>
    /// <param name="analyzer">
    ///     The index analyzer.
    /// </param>
    /// <param name="fileSystemProvider">
    ///     The file system service.
    /// </param>
    /// <param name="options">
    ///     The Lucene options.
    /// </param>
    /// <param name="logger">
    ///     The logger.
    /// </param>
    /// <param name="name">
    ///     The name of the index. If not provided, will use a default name.
    ///     Default name is <see cref="LuceneDefaults.IndexName"/>.
    /// </param>
    public Index(
        Analyzer analyzer,
        IFileExplorer fileSystemProvider,
        string name,
        IOptions<LuceneOptions> options,
        ILogger<Index> logger
    ) {
        _analyzer = analyzer;
        _fileSystemProvider = fileSystemProvider;
        _options = options;
        _logger = logger;

        _indexWriterConfig = new Lazy<IndexWriterConfig>(CreateIndexWriterConfig);
        _fsDirectory = new Lazy<FSDirectory>(CreateFSDirectory);

        Name = name;
    }

    /// <inheritdoc />
    public Result<bool> Insert(DocumentCollection documents) {
        BlockAccessAfterDispose();

        try {
            GetIndexWriter().AddDocuments(documents);

            return true;
        }
        catch (Exception ex) {
            if (ex is OutOfMemoryException) { DestroyIndexWriter(); }

            CommonLog.Failure(_logger, ex, tag: LogTag);

            return Error.Failure(ex.Message);
        }
    }

    /// <inheritdoc />
    public Result<bool> Delete(Query query) {
        BlockAccessAfterDispose();

        try {
            GetIndexWriter().DeleteDocuments(query);

            return true;
        }
        catch (Exception ex) {
            if (ex is OutOfMemoryException) { DestroyIndexWriter(); }

            CommonLog.Failure(_logger, ex, tag: LogTag);

            return Error.Failure(ex.Message);
        }
    }

    /// <inheritdoc />
    public Result<bool> Update(Term term, Document document) {
        BlockAccessAfterDispose();

        try {
            GetIndexWriter().UpdateDocument(term, document);

            return true;
        }
        catch (Exception ex) {
            if (ex is OutOfMemoryException) { DestroyIndexWriter(); }

            CommonLog.Failure(_logger, ex, tag: LogTag);

            return Error.Failure(ex.Message);
        }
    }

    /// <inheritdoc />
    public IEnumerable<ScoreDocument> Search(Query query, Sort sort, int limit) {
        BlockAccessAfterDispose();

        try {
            var indexReader = GetIndexReader();

            return new SearchEnumerable(
                new IndexSearcher(indexReader),
                query,
                sort,
                limit
            );
        }
        catch (Exception ex) {
            CommonLog.Failure(_logger, ex, tag: LogTag);

            throw;
        }
    }

    /// <inheritdoc />
    public Result<int> Count(Query query) {
        BlockAccessAfterDispose();

        try {
            var indexReader = GetIndexReader();
            var collector = new TotalHitCountCollector();

            new IndexSearcher(indexReader).Search(query, collector);

            return collector.TotalHits;
        }
        catch (Exception ex) {
            CommonLog.Failure(_logger, ex, tag: LogTag);

            return Error.Failure(ex.Message);
        }
    }

    /// <inheritdoc />
    public Result<bool> Rollback() {
        BlockAccessAfterDispose();

        try {
            GetIndexWriter().Rollback();

            return true;
        }
        catch (Exception ex) {
            CommonLog.Failure(_logger, ex, tag: LogTag);

            return Error.Failure(ex.Message);
        }
    }

    /// <inheritdoc />
    public Result<bool> SaveChanges() {
        BlockAccessAfterDispose();

        try {
            GetIndexWriter().Commit();

            return true;
        }
        catch (Exception ex) {
            CommonLog.Failure(_logger, ex, tag: LogTag);

            return Error.Failure(ex.Message);
        }
    }

    /// <inheritdoc />
    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Registers a dispose callback.
    /// </summary>
    /// <param name="callback">
    ///     The callback.
    /// </param>
    public void RegisterDisposeCallback(Action<Index> callback) {
        BlockAccessAfterDispose();

        _disposeCallback += callback;
    }

    /// <summary>
    ///     Unregisters the dispose callback.
    /// </summary>
    /// <param name="callback">
    ///     The callback.
    /// </param>
    public void UnregisterDisposeCallback(Action<Index> callback) {
        BlockAccessAfterDispose();

        _disposeCallback -= callback;
    }

    private void BlockAccessAfterDispose() {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    /// <summary>
    ///     Releases the managed and unmanaged resources used by this instance.
    /// </summary>
    /// <param name="disposing">
    ///     <see langword="true"/> to release both managed and unmanaged resources;
    ///     <see langword="false"/> to release only unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing) {
        if (_disposed) { return; }

        if (disposing) {
            _indexWriter?.Dispose();
            _indexReader?.Dispose();

            if (_fsDirectory?.IsValueCreated is true) {
                _fsDirectory.Value.Dispose();
            }
        }

        _disposeCallback?.Invoke(this);

        _indexWriter = null;
        _indexReader = null;
        _indexWriterConfig = null;
        _fsDirectory = null;
        _disposeCallback = null;

        _disposed = true;
    }

    private IndexWriterConfig CreateIndexWriterConfig() {
        return new IndexWriterConfig(LuceneDefaults.Version, _analyzer);
    }

    private FSDirectory CreateFSDirectory() {
        try {
            var relativeDirectoryPath = Path.Combine(_options.Value.DirectoryName, Name);
            var directory = _fileSystemProvider.GetDirectory(relativeDirectoryPath);

            // Ensure directory existence.
            directory.Create();

            return FSDirectory.Open(directory.Path);
        }
        catch (Exception ex) {
            CommonLog.Failure(_logger, ex, tag: LogTag);

            throw;
        }
    }

    private IndexWriter GetIndexWriter() {
        return _indexWriter ??= new IndexWriter(FSDirectory, IndexWriterConfig);
    }

    private IndexReader GetIndexReader() {
        if (_indexReader is DirectoryReader currentDirectoryReader) {
            var newIndexReader = DirectoryReader.OpenIfChanged(currentDirectoryReader);
            if (newIndexReader is not null) {
                // This is necessary to refresh the index reader
                _indexReader.Dispose();
                _indexReader = null;

                _indexReader = newIndexReader;
            }
        }

        try { return _indexReader ??= DirectoryReader.Open(FSDirectory); }
        catch (IndexNotFoundException) {
            // Most cases where this exception is thrown, are
            // those where the index was not yet written, it is "empty".
            return EmptyIndexReader.Instance;
        }
        catch (Exception ex) {
            CommonLog.Failure(_logger, ex, tag: LogTag);

            throw;
        }
    }

    private void DestroyIndexWriter() {
        try {
            _indexWriter?.Dispose();
            _indexWriter = null;
        }
        catch { /* swallow */ }
    }
}
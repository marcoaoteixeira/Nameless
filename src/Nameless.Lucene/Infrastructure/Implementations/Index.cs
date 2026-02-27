#pragma warning disable CA1859
// ReSharper disable InconsistentNaming

using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.IO.FileSystem;
using Nameless.Lucene.Empty;
using Nameless.Lucene.Internals;
using Nameless.Lucene.ObjectModel;
using Nameless.ObjectModel;
using Nameless.Results;

namespace Nameless.Lucene.Infrastructure.Implementations;

/// <summary>
///     Default implementation of <see cref="IIndex"/>.
/// </summary>
public class Index : IIndex {
    private readonly Analyzer _analyzer;
    private readonly IFileSystem _fileSystem;
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
    /// <param name="fileSystem">
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
    ///     Default name is <see cref="Defaults.IndexName"/>.
    /// </param>
    public Index(
        Analyzer analyzer,
        IFileSystem fileSystem,
        string name,
        IOptions<LuceneOptions> options,
        ILogger<Index> logger
    ) {
        _analyzer = analyzer;
        _fileSystem = fileSystem;
        _options = options;
        _logger = logger;

        _indexWriterConfig = new Lazy<IndexWriterConfig>(CreateIndexWriterConfig);
        _fsDirectory = new Lazy<FSDirectory>(CreateFSDirectory);

        Name = name;
    }

    /// <inheritdoc />
    public Result<int> Insert(DocumentCollection documents) {
        BlockAccessAfterDispose();

        try {
            GetIndexWriter().AddDocuments(documents);

            return documents.Count;
        }
        catch (Exception ex) {
            if (ex is OutOfMemoryException) { DestroyIndexWriter(); }

            _logger.InsertDocumentsFailure(Name, ex);

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

            _logger.DeleteDocumentsFailure(Name, ex);

            return Error.Failure(ex.Message);
        }
    }

    /// <inheritdoc />
    public Result<DocumentCollection> Search(Query query, ICollector collector) {
        BlockAccessAfterDispose();

        try {
            var indexReader = GetIndexReader();
            var indexSearcher = new IndexSearcher(indexReader);

            indexSearcher.Search(query, collector);

            return CollectDocuments(collector, indexSearcher);
        }
        catch (Exception ex) {
            if (ex is OutOfMemoryException) { DestroyIndexWriter(); }

            _logger.SearchDocumentsFailure(Name, ex);

            return Error.Failure(ex.Message);
        }
    }

    /// <inheritdoc />
    public Result<int> Count(Query query) {
        BlockAccessAfterDispose();

        try {
            var indexReader = GetIndexReader();
            var indexSearcher = new IndexSearcher(indexReader);
            var collector = new TotalHitCountCollector();

            indexSearcher.Search(query, collector);

            return collector.TotalHits;
        }
        catch (Exception ex) {
            _logger.CountDocumentsFailure(Name, ex);

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
            _logger.SaveChangesFailure(Name, ex);

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
        _disposeCallback += callback;
    }

    /// <summary>
    ///     Unregisters the dispose callback.
    /// </summary>
    /// <param name="callback">
    ///     The callback.
    /// </param>
    public void UnregisterDisposeCallback(Action<Index> callback) {
        _disposeCallback -= callback;
    }

    private static Result<DocumentCollection> CollectDocuments(ICollector collector, IndexSearcher searcher) {
        return collector switch {
            TopFieldCollector topCollector => topCollector.Collect(searcher),
            _ => Error.Missing($"Document collector not implemented for '{collector.GetType().Name}'.")
        };
    }

    private void BlockAccessAfterDispose() {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

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
        return new IndexWriterConfig(Defaults.CurrentVersion, _analyzer);
    }

    private FSDirectory CreateFSDirectory() {
        try {
            var relativeDirectoryPath = Path.Combine(_options.Value.DirectoryName, Name);
            var directory = _fileSystem.GetDirectory(relativeDirectoryPath);

            // Ensure directory existence.
            directory.Create();

            return FSDirectory.Open(directory.Path);
        }
        catch (Exception ex) {
            _logger.CreateFSDirectoryFailure(Name, ex);

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
            _logger.GetIndexReaderFailure(Name, ex);

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
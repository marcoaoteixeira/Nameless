#pragma warning disable CA1859

using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nameless.IO.FileSystem;
using Nameless.Lucene.Empty;
using Nameless.Lucene.Internals;
using Nameless.Lucene.Requests;
using Nameless.Lucene.Responses;
using Nameless.ObjectModel;

namespace Nameless.Lucene;

/// <summary>
///     Default implementation of <see cref="IIndex"/>.
/// </summary>
public class Index : IIndex {
    private const string USER_CANCELLED_TASK_ERROR = "The operation was cancelled by the user.";
    private const string MAX_BOOLEAN_CLAUSE_EXCEEDED_ERROR = "The number of clauses in the boolean query exceeds the maximum allowed (1024).";

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

    /// <summary>
    ///     Gets the name of the index.
    /// </summary>
    public string Name { get; }

    private FSDirectory FSDirectory => _fsDirectory?.Value ??
                                       throw new InvalidOperationException($"{nameof(FSDirectory)} not available.");

    private IndexWriterConfig IndexWriterConfig => _indexWriterConfig?.Value ??
                                                   throw new InvalidOperationException(
                                                       $"{nameof(IndexWriterConfig)} not available.");

    public Index(Analyzer analyzer, IFileSystem fileSystem, string name, IOptions<LuceneOptions> options, ILogger<Index> logger) {
        _analyzer = analyzer;
        _fileSystem = fileSystem;
        _options = options;
        _logger = logger;

        _indexWriterConfig = new Lazy<IndexWriterConfig>(CreateIndexWriterConfig);
        _fsDirectory = new Lazy<FSDirectory>(CreateFSDirectory);

        Name = name;
    }

    ~Index() {
        Dispose(disposing: false);
    }

    /// <inheritdoc />
    public IQueryBuilder CreateQueryBuilder() {
        BlockAccessAfterDispose();

        return new QueryBuilder(_analyzer);
    }

    /// <inheritdoc />
    public Task<InsertResponse> InsertAsync(InsertDocumentsRequest documentsRequest, CancellationToken cancellationToken) {
        BlockAccessAfterDispose();
        
        try {
            var docs = documentsRequest.Documents
                              .TakeWhile(_ => !cancellationToken.IsCancellationRequested)
                              .Select(document => document.ToIndexableFields())
                              .ToArray();

            if (cancellationToken.IsCancellationRequested) {
                return InsertResponse.From(count: 0);
            }

            var indexWriter = GetIndexWriter();

            indexWriter.AddDocuments(docs);
            indexWriter.Commit();

            return InsertResponse.From(documentsRequest.Documents.Length);
        }
        catch (Exception ex) {
            if (ex is OutOfMemoryException) { DestroyIndexWriter(); }

            _logger.InsertDocumentsFailure(ex);

            return InsertResponse.From(Error.Failure(ex.Message));
        }
    }

    /// <inheritdoc />
    public Task<DeleteDocumentsResponse> DeleteAsync(DeleteDocumentsRequest request, CancellationToken cancellationToken) {
        BlockAccessAfterDispose();

        if (request.Documents.Length > BooleanQuery.MaxClauseCount) {
            return DeleteDocumentsResponse.From(Error.Failure(MAX_BOOLEAN_CLAUSE_EXCEEDED_ERROR));
        }

        try {
            var clauses = request.Documents
                                 .TakeWhile(_ => !cancellationToken.IsCancellationRequested)
                                 .Select(document => document.CreateDeleteBooleanClause())
                                 .ToArray();

            if (cancellationToken.IsCancellationRequested) {
                return DeleteDocumentsResponse.From(Error.Failure(USER_CANCELLED_TASK_ERROR));
            }

            var query = new BooleanQuery();
            query.Clauses.AddRange(clauses);

            var indexWriter = GetIndexWriter();
            indexWriter.DeleteDocuments(query);
            indexWriter.Commit();

            return DeleteDocumentsResponse.From(request.Documents.Length);
        }
        catch (Exception ex) {
            if (ex is OutOfMemoryException) { DestroyIndexWriter(); }

            _logger.DeleteDocumentsFailure(ex);

            return DeleteDocumentsResponse.From(Error.Failure(ex.Message));
        }
    }

    /// <inheritdoc />
    public Task<DeleteDocumentsByQueryResponse> DeleteAsync(DeleteDocumentsByQueryRequest request, CancellationToken cancellationToken) {
        BlockAccessAfterDispose();

        try {
            var count = GetQueryCount(request.Query);

            if (count == 0) {
                return DeleteDocumentsByQueryResponse.From(count);
            }

            if (cancellationToken.IsCancellationRequested) {
                return DeleteDocumentsByQueryResponse.From(Error.Failure(USER_CANCELLED_TASK_ERROR));
            }

            var indexWriter = GetIndexWriter();

            indexWriter.DeleteDocuments(request.Query);
            indexWriter.Commit();

            return DeleteDocumentsByQueryResponse.From(count);
        }
        catch (Exception ex) {
            if (ex is OutOfMemoryException) { DestroyIndexWriter(); }

            _logger.DeleteDocumentsFailure(ex);

            return DeleteDocumentsByQueryResponse.From(Error.Failure(ex.Message));
        }
    }

    /// <inheritdoc />
    public Task<SearchDocumentsResponse> SearchAsync(SearchDocumentsRequest request, CancellationToken cancellationToken) {
        BlockAccessAfterDispose();

        try {
            var count = GetQueryCount(request.Query);

            if (count == 0) {
                return SearchDocumentsResponse.From(hits: [], count);
            }

            if (cancellationToken.IsCancellationRequested) {
                return SearchDocumentsResponse.From(Error.Failure(USER_CANCELLED_TASK_ERROR));
            }

            var collector = TopFieldCollector.Create(
                request.Sort,
                request.Start + request.Limit,
                fillFields: false,
                trackDocScores: true,
                trackMaxScore: false,
                docsScoredInOrder: true
            );
            var indexReader = GetIndexReader();
            var indexSearch = new IndexSearcher(indexReader);

            indexSearch.Search(request.Query, collector);

            var hits = collector.GetTopDocs()
                                .ScoreDocs
                                .Select(score => score.ToSearchHit(indexSearch))
                                .ToArray();

            return SearchDocumentsResponse.From(hits, count);
        }
        catch (Exception ex) {
            _logger.SearchFailure(ex);

            return SearchDocumentsResponse.From(Error.Failure(ex.Message));
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
        _disposeCallback += Guard.Against.Null(callback);
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

    private void BlockAccessAfterDispose() {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    private void Dispose(bool disposing) {
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
        return new IndexWriterConfig(Constants.CURRENT_VERSION, _analyzer);
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
            _logger.CreateFSDirectoryFailure(ex);

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

        try {
            return _indexReader ??= DirectoryReader.Open(FSDirectory);
        }
        catch (IndexNotFoundException) {
            // Most cases where this exception is thrown, are
            // those where the index was not yet written, it is "empty".
            return EmptyIndexReader.Instance;
        }
        catch (Exception ex) {
            _logger.GetIndexReaderFailure(ex);

            throw;
        }
    }

    private void DestroyIndexWriter() {
        try {
            _indexWriter?.Dispose();
            _indexWriter = null;
        }
        catch {
            /* swallow */
        }
    }

    private int GetQueryCount(Query query) {
        try {
            var indexReader = GetIndexReader();
            var indexSearcher = new IndexSearcher(indexReader);
            var collector = new TotalHitCountCollector();

            indexSearcher.Search(query, collector);

            return collector.TotalHits;
        }
        catch (Exception ex) { _logger.GetQueryCountFailure(ex); }

        return 0;
    }
}
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
using Nameless.Lucene.Results;

namespace Nameless.Lucene;

/// <summary>
///     Default implementation of <see cref="IIndex"/>.
/// </summary>
public sealed class Index : IIndex {
    private const int INSERT_CHUNK_SIZE = 32;
    private const int REMOVE_CHUNK_SIZE = 32;

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

    private FSDirectory FSDirectory => _fsDirectory?.Value ?? throw new InvalidOperationException($"{nameof(FSDirectory)} not available.");

    private IndexWriterConfig IndexWriterConfig => _indexWriterConfig?.Value ?? throw new InvalidOperationException($"{nameof(IndexWriterConfig)} not available.");

    public Index(Analyzer analyzer, IFileSystem fileSystem, string name, IOptions<LuceneOptions> options, ILogger<Index> logger) {
        _analyzer = Guard.Against.Null(analyzer);
        _fileSystem = Guard.Against.Null(fileSystem);
        _options = Guard.Against.Null(options);
        _logger = Guard.Against.Null(logger);

        _indexWriterConfig = new Lazy<IndexWriterConfig>(CreateIndexWriterConfig);
        _fsDirectory = new Lazy<FSDirectory>(CreateFSDirectory);

        Name = Guard.Against.NullOrWhiteSpace(name);
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
    public Task<InsertDocumentsResult> InsertAsync(IDocument[] documents, CancellationToken cancellationToken) {
        BlockAccessAfterDispose();

        Guard.Against.NullOrEmpty(documents);

        InsertDocumentsResult result;
        var chunks = documents.Chunk(INSERT_CHUNK_SIZE);
        var count = 0;

        try {
            var indexWriter = GetIndexWriter();

            foreach (var chunk in chunks) {
                if (cancellationToken.IsCancellationRequested) {
                    break;
                }

                count += chunk.Length;

                var docs = chunk.Select(document => document.ToDocument());

                indexWriter.AddDocuments(docs);
                indexWriter.Commit();
            }

            result = InsertDocumentsResult.Success(
                count,
                cancellationToken.IsCancellationRequested
            );
        }
        catch (OutOfMemoryException ex) {
            DestroyIndexWriter();

            _logger.InsertDocumentsFailure(ex);

            result = InsertDocumentsResult.Failure(ex.Message);
        }
        catch (Exception ex) {
            _logger.InsertDocumentsFailure(ex);

            result = InsertDocumentsResult.Failure(ex.Message);
        }

        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public Task<RemoveDocumentsResult> RemoveAsync(IDocument[] documents, CancellationToken cancellationToken) {
        BlockAccessAfterDispose();

        Guard.Against.NullOrEmpty(documents);

        RemoveDocumentsResult result;

        // Chunks cannot be larger than the maximum number of
        // clauses permitted in a boolean query.
        // See BooleanQuery.MaxClauseCount
        var chunks = documents.Chunk(REMOVE_CHUNK_SIZE);
        var count = 0;

        try {
            var indexWriter = GetIndexWriter();

            foreach (var chunk in chunks) {
                if (cancellationToken.IsCancellationRequested) {
                    break;
                }

                count += chunk.Length;

                var query = new BooleanQuery();
                var clauses = chunk.Select(CreateDeleteClause);

                query.Clauses.AddRange(clauses);

                indexWriter.DeleteDocuments(query);
                indexWriter.Commit();
            }

            result = RemoveDocumentsResult.Success(
                count,
                cancellationToken.IsCancellationRequested
            );
        }
        catch (OutOfMemoryException ex) {
            DestroyIndexWriter();

            _logger.RemoveDocumentsFailure(ex);

            result = RemoveDocumentsResult.Failure(ex.Message);
        }
        catch (Exception ex) {
            _logger.RemoveDocumentsFailure(ex);

            result = RemoveDocumentsResult.Failure(ex.Message);
        }

        return Task.FromResult(result);

        static BooleanClause CreateDeleteClause(IDocument doc) {
            var term = new Term(nameof(IDocument.ID), doc.ID);
            var termQuery = new TermQuery(term);

            return new BooleanClause(termQuery, Occur.SHOULD);
        }
    }

    /// <inheritdoc />
    public Task<RemoveDocumentsResult> RemoveAsync(Query query, CancellationToken cancellationToken) {
        BlockAccessAfterDispose();

        Guard.Against.Null(query);

        RemoveDocumentsResult result;

        try {
            var count = GetQueryCount(query);
            var indexWriter = GetIndexWriter();

            indexWriter.DeleteDocuments(query);
            indexWriter.Commit();

            result = RemoveDocumentsResult.Success(
                count,
                cancellationToken.IsCancellationRequested
            );
        }
        catch (OutOfMemoryException ex) {
            DestroyIndexWriter();

            _logger.RemoveDocumentsFailure(ex);

            result = RemoveDocumentsResult.Failure(ex.Message);
        }
        catch (Exception ex) {
            _logger.RemoveDocumentsFailure(ex);

            result = RemoveDocumentsResult.Failure(ex.Message);
        }

        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public Task<SearchDocumentsResult> SearchAsync(Query query, Sort sort, int start, int limit, CancellationToken cancellationToken) {
        BlockAccessAfterDispose();

        Guard.Against.Null(query);
        Guard.Against.Null(sort);
        Guard.Against.LowerThan(start, compare: 0);
        Guard.Against.LowerOrEqual(limit, compare: 0);

        SearchDocumentsResult result;

        try {
            var count = GetQueryCount(query);

            var collector = TopFieldCollector.Create(
                sort,
                numHits: start + limit,
                fillFields: false,
                trackDocScores: true,
                trackMaxScore: false,
                docsScoredInOrder: true
            );
            var indexReader = GetIndexReader();
            var indexSearch = new IndexSearcher(indexReader);

            indexSearch.Search(query, collector);

            var hits = collector.GetTopDocs()
                                .ScoreDocs
                                .Select(score => score.ToSearchHit(indexSearch))
                                .ToArray();

            result = SearchDocumentsResult.Success(count, hits);
        }
        catch (Exception ex) {
            _logger.SearchFailure(ex);

            result = SearchDocumentsResult.Failure(ex.Message);
        }

        return Task.FromResult(result);
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
        catch { /* swallow */ }
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
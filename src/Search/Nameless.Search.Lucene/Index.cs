using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Microsoft.Extensions.Logging;

namespace Nameless.Search.Lucene;

/// <summary>
/// Default implementation of <see cref="IIndex"/>
/// </summary>
public sealed class Index : IIndex, IDisposable {
    private static int BatchSize => BooleanQuery.MaxClauseCount;

    private readonly Analyzer _analyzer;
    private readonly ILogger _logger;
    private readonly IndexWriterConfig _indexWriterConfig;

    private FSDirectory? _fsDirectory;
    private IndexReader? _indexReader;
    private IndexWriter? _indexWriter;

    private bool _disposed;

    /// <inheritdoc />
    public string Name { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="Index"/>.
    /// </summary>
    /// <param name="analyzer">The Lucene analyzer.</param>
    /// <param name="indexDirectoryPath">The index directory path.</param>
    /// <param name="indexName">The index name.</param>
    /// <param name="logger">The <see cref="ILogger{TCategory}"/> object.</param>
    public Index(Analyzer analyzer, string indexDirectoryPath, string indexName, ILogger<Index> logger) {
        Prevent.Argument.NullOrWhiteSpace(indexDirectoryPath);

        _analyzer = Prevent.Argument.Null(analyzer);
        _logger = Prevent.Argument.Null(logger);
        _indexWriterConfig = new IndexWriterConfig(Defaults.Version, _analyzer);

        _fsDirectory = FSDirectory.Open(path: new DirectoryInfo(indexDirectoryPath));

        Name = Prevent.Argument.NullOrWhiteSpace(indexName);
    }

    ~Index() {
        Dispose(disposing: false);
    }

    /// <inheritdoc />
    public bool IsEmpty() {
        BlockAccessAfterDispose();

        return Count() <= 0;
    }

    /// <inheritdoc />
    public int Count() {
        BlockAccessAfterDispose();

        return GetIndexReader().NumDocs;
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="documentID"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// if <paramref name="documentID"/> is empty or white spaces.
    /// </exception>
    public IDocument NewDocument(string documentID) {
        BlockAccessAfterDispose();

        Prevent.Argument.NullOrWhiteSpace(documentID);

        return new Document(documentID);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="documents"/> is <c>null</c>.
    /// </exception>
    public async Task<IndexActionResult> StoreDocumentsAsync(IDocument[] documents, CancellationToken cancellationToken) {
        BlockAccessAfterDispose();

        Prevent.Argument.Null(documents);

        if (documents.Length == 0) {
            return IndexActionResult.Success(total: 0);
        }

        var result = await InnerDeleteDocumentsAsync(documents, cancellationToken);
        if (!result.Succeeded) {
            return result;
        }

        result = await InnerStoreDocumentsAsync(documents, cancellationToken);

        return result.Succeeded
            ? InnerCommitChanges(result.Total)
            : result;
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="documents"/> is <c>null</c>.
    /// </exception>
    public async Task<IndexActionResult> DeleteDocumentsAsync(IDocument[] documents, CancellationToken cancellationToken) {
        BlockAccessAfterDispose();

        Prevent.Argument.Null(documents);

        if (documents.Length == 0) {
            return IndexActionResult.Success(total: 0);
        }

        var result = await InnerDeleteDocumentsAsync(documents, cancellationToken);

        return result.Succeeded
            ? InnerCommitChanges(result.Total)
            : result;
    }

    /// <inheritdoc />
    public ISearchBuilder CreateSearchBuilder()
        => new SearchBuilder(_analyzer, GetIndexReader());

    /// <inheritdoc />
    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private Task<IndexActionResult> InnerStoreDocumentsAsync(IDocument[] documents, CancellationToken cancellationToken) {
        IndexActionResult result;

        var counter = 0;

        try {
            var indexWriter = GetIndexWriter();

            foreach (var document in documents) {
                cancellationToken.ThrowIfCancellationRequested();

                indexWriter.AddDocument(doc: document.ToDocument());
                counter++;
            }

            result = IndexActionResult.Success(counter);
        } catch (Exception ex) {
            result = CreateFailureIndexActionResult(ex, ActionType.Store);
        }

        return Task.FromResult(result);
    }

    private Task<IndexActionResult> InnerDeleteDocumentsAsync(IDocument[] documents, CancellationToken cancellationToken) {
        var counter = 0;
        var batches = documents.Length / BatchSize;
        if (documents.Length % BatchSize != 0) {
            ++batches;
        }

        IndexActionResult result;

        try {
            for (var batch = 0; batch < batches; batch++) {
                cancellationToken.ThrowIfCancellationRequested();

                var query = new BooleanQuery();
                var documentBatch = documents.Skip(batch * BatchSize)
                                             .Take(BatchSize);

                foreach (var document in documentBatch) {
                    var term = new Term(fld: nameof(ISearchHit.DocumentID), text: document.ID);
                    var termQuery = new TermQuery(term);
                    var booleanClause = new BooleanClause(query: termQuery, occur: Occur.SHOULD);

                    query.Add(booleanClause);

                    counter++;
                }

                GetIndexWriter().DeleteDocuments(query);
            }

            result = IndexActionResult.Success(counter);
        } catch (Exception ex) {
            result = CreateFailureIndexActionResult(ex, ActionType.Delete);
        }

        return Task.FromResult(result);
    }

    private IndexActionResult InnerCommitChanges(int totalDocuments) {
        try {
            var indexWriter = GetIndexWriter();
            indexWriter.PrepareCommit();
            indexWriter.Commit();

            return IndexActionResult.Success(totalDocuments);
        } catch (Exception ex) { return CreateFailureIndexActionResult(ex, ActionType.Commit); }
    }

    private IndexActionResult CreateFailureIndexActionResult(Exception ex, ActionType type) {
        return ex switch {
            OperationCanceledException => OperationCanceledExceptionHandler(),
            OutOfMemoryException => OutOfMemoryExceptionHandler(ex),
            _ => UnknownExceptionHandler(ex, type)
        };

        IndexActionResult OperationCanceledExceptionHandler() {
            return IndexActionResult.Failure(Constants.Messages.USER_CANCELLED_TASK);
        }

        IndexActionResult OutOfMemoryExceptionHandler(Exception exception) {
            _logger.IndexWriterOutOfMemoryError(exception);

            DestroyIndexWriter();

            return IndexActionResult.Failure(Constants.Messages.LUCENE_OUT_OF_MEMORY);
        }

        IndexActionResult UnknownExceptionHandler(Exception exception, ActionType actionType) {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (actionType) {
                case ActionType.Store:
                    _logger.StoreDocumentsUnhandledError(exception);
                    break;
                case ActionType.Delete:
                    _logger.DeleteDocumentsUnhandledError(exception);
                    break;
                case ActionType.Commit:
                    _logger.CommitChangesUnhandledError(exception);
                    break;
            }

            return IndexActionResult.Failure(Constants.Messages.UNHANDLED_EXCEPTION);
        }
    }

    private IndexWriter GetIndexWriter()
        => _indexWriter ??= new IndexWriter(_fsDirectory, _indexWriterConfig);

    private void DestroyIndexWriter() {
        try {
            _indexWriter?.Dispose();
            _indexWriter = null;
        } catch { /* ignore */ }
    }

    private IndexReader GetIndexReader() {
        if (_indexReader is DirectoryReader currentDirectoryReader) {
            var newIndexReader = DirectoryReader.OpenIfChanged(currentDirectoryReader);
            if (newIndexReader is not null) {
                _indexReader.Dispose();
                _indexReader = null;

                _indexReader = newIndexReader;
            }
        }

        try { return _indexReader ??= DirectoryReader.Open(_fsDirectory); }
        catch (IndexNotFoundException) { return new EmptyIndexReader(); }
    }

    private void BlockAccessAfterDispose() {
#if NET8_0_OR_GREATER
        ObjectDisposedException.ThrowIf(_disposed, this);
#else
        if (_disposed) {
            throw new ObjectDisposedException(nameof(Index));
        }
#endif
    }

    private void Dispose(bool disposing) {
        if (_disposed) { return; }

        if (disposing) {
            _indexWriter?.Dispose();
            _indexReader?.Dispose();
            _fsDirectory?.Dispose();
        }

        _indexWriter = null;
        _indexReader = null;
        _fsDirectory = null;

        _disposed = true;
    }

    private enum ActionType {
        Store,

        Delete,

        Commit
    }
}
﻿#pragma warning disable CA1859 // IndexReader is not assignable from AtomicReader

using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Microsoft.Extensions.Logging;
using Nameless.Search.Lucene.Null;

namespace Nameless.Search.Lucene;

/// <summary>
///     Default implementation of <see cref="IIndex" />
/// </summary>
public sealed class Index : IIndex {
    private static int BatchSize => BooleanQuery.MaxClauseCount;

    private readonly Analyzer _analyzer;
    private readonly IndexWriterConfig _indexWriterConfig;
    private readonly ILogger _logger;

    private FSDirectory? _fsDirectory;
    private IndexReader? _indexReader;
    private IndexWriter? _indexWriter;

    private bool _disposed;

    /// <inheritdoc />
    public string Name { get; }

    public event EventHandler<DisposeIndexEventArgs>? OnDispose;

    /// <summary>
    ///     Initializes a new instance of <see cref="Index" />.
    /// </summary>
    /// <param name="analyzer">The Lucene analyzer.</param>
    /// <param name="indexDirectoryPath">The index directory path.</param>
    /// <param name="indexName">The index name.</param>
    /// <param name="logger">The <see cref="ILogger{TCategory}" /> object.</param>
    public Index(Analyzer analyzer,
                 string indexDirectoryPath,
                 string indexName,
                 ILogger<Index> logger) {
        Prevent.Argument.NullOrWhiteSpace(indexDirectoryPath);

        _analyzer = Prevent.Argument.Null(analyzer);
        _logger = Prevent.Argument.Null(logger);
        _indexWriterConfig = new IndexWriterConfig(Defaults.Version, _analyzer);
        _fsDirectory = FSDirectory.Open(new DirectoryInfo(indexDirectoryPath));

        Name = Prevent.Argument.NullOrWhiteSpace(indexName);
    }

    ~Index() {
        Dispose(false);
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
    ///     if <paramref name="documentID" /> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     if <paramref name="documentID" /> is empty or white spaces.
    /// </exception>
    public IDocument NewDocument(string documentID) {
        BlockAccessAfterDispose();

        Prevent.Argument.NullOrWhiteSpace(documentID);

        return new Document(documentID);
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="documents" /> is <see langword="null"/>.
    /// </exception>
    public async Task<IndexActionResult> StoreDocumentsAsync(IDocument[] documents, CancellationToken cancellationToken) {
        BlockAccessAfterDispose();

        Prevent.Argument.Null(documents);

        if (documents.Length == 0) {
            return IndexActionResult.Success(totalDocumentsAffected: 0);
        }

        var result = await InnerDeleteDocumentsAsync(documents, cancellationToken);
        if (!result.Succeeded) {
            return result;
        }

        result = await InnerStoreDocumentsAsync(documents, cancellationToken);

        return result.Succeeded
            ? InnerCommitChanges(result.TotalDocumentsAffected)
            : result;
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    ///     if <paramref name="documents" /> is <see langword="null"/>.
    /// </exception>
    public async Task<IndexActionResult> DeleteDocumentsAsync(IDocument[] documents, CancellationToken cancellationToken) {
        BlockAccessAfterDispose();

        Prevent.Argument.Null(documents);

        if (documents.Length == 0) {
            return IndexActionResult.Success(0);
        }

        var result = await InnerDeleteDocumentsAsync(documents, cancellationToken);

        return result.Succeeded
            ? InnerCommitChanges(result.TotalDocumentsAffected)
            : result;
    }

    /// <inheritdoc />
    public ISearchBuilder CreateSearchBuilder() {
        return new SearchBuilder(_analyzer, GetIndexReader());
    }

    /// <inheritdoc />
    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private Task<IndexActionResult> InnerStoreDocumentsAsync(IDocument[] documents, CancellationToken cancellationToken) {
        IndexActionResult result;

        var counter = 0;

        try {
            var indexWriter = GetIndexWriter();

            foreach (var document in documents) {
                cancellationToken.ThrowIfCancellationRequested();

                indexWriter.AddDocument(document.ToDocument());
                counter++;
            }

            result = IndexActionResult.Success(counter);
        }
        catch (Exception ex) {
            result = CreateFailureIndexActionResult(ex, ActionType.Store);
        }

        return Task.FromResult(result);
    }

    private Task<IndexActionResult> InnerDeleteDocumentsAsync(IDocument[] documents, CancellationToken cancellationToken) {
        var counter = 0;
        var batches = (int)Math.Ceiling(documents.Length / (double)BatchSize);

        IndexActionResult result;

        try {
            for (var batch = 0; batch < batches; batch++) {
                cancellationToken.ThrowIfCancellationRequested();

                var query = new BooleanQuery();
                var documentBatch = documents.Skip(batch * BatchSize)
                                             .Take(BatchSize);

                foreach (var document in documentBatch) {
                    var term = new Term(nameof(ISearchHit.DocumentID), document.ID);
                    var termQuery = new TermQuery(term);
                    var booleanClause = new BooleanClause(termQuery, Occur.SHOULD);

                    query.Add(booleanClause);

                    counter++;
                }

                GetIndexWriter().DeleteDocuments(query);
            }

            result = IndexActionResult.Success(counter);
        }
        catch (Exception ex) {
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
        }
        catch (Exception ex) { return CreateFailureIndexActionResult(ex, ActionType.Commit); }
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

    private IndexWriter GetIndexWriter() {
        return _indexWriter ??= new IndexWriter(_fsDirectory, _indexWriterConfig);
    }

    private void DestroyIndexWriter() {
        try {
            _indexWriter?.Dispose();
            _indexWriter = null;
        }
        catch { /* ignore */ }
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

        try { return _indexReader ??= DirectoryReader.Open(_fsDirectory); }
        catch (IndexNotFoundException) { return NullIndexReader.Instance; }
    }

    private void BlockAccessAfterDispose() {
        ObjectDisposedException.ThrowIf(_disposed, this);
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

        OnDispose?.Invoke(this, new DisposeIndexEventArgs(Name));
    }

    private enum ActionType {
        Store,

        Delete,

        Commit
    }
}
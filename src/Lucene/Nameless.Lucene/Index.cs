using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Microsoft.Extensions.Logging;
using Nameless.Lucene.Internals;

namespace Nameless.Lucene;

/// <summary>
/// Default implementation of <see cref="IIndex"/>
/// </summary>
public sealed class Index : IIndex, IDisposable {
    private readonly Analyzer _analyzer;
    private readonly string _indexDirectoryPath;
    private readonly ILogger _logger;
    private readonly IndexWriterConfig _indexWriterConfig;

    private FSDirectory? _fsDirectory;
    private IndexReader? _indexReader;
    private IndexWriter? _indexWriter;

    private bool _disposed;

    private static int BatchSize => BooleanQuery.MaxClauseCount;

    /// <inheritdoc />
    public string Name { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="Index"/>.
    /// </summary>
    /// <param name="analyzer">The Lucene analyzer.</param>
    /// <param name="indexDirectoryPath">The base path of the Lucene directory.</param>
    /// <param name="logger">The <see cref="ILogger{Index}"/> instance.</param>
    public Index(Analyzer analyzer, string indexDirectoryPath, ILogger<Index> logger) {
        _analyzer = Prevent.Argument.Null(analyzer);
        _indexDirectoryPath = Prevent.Argument.NullOrWhiteSpace(indexDirectoryPath);
        _logger = Prevent.Argument.Null(logger);

        _fsDirectory = FSDirectory.Open(path: new DirectoryInfo(_indexDirectoryPath));
        _indexWriterConfig = new IndexWriterConfig(Internals.Defaults.Version, _analyzer);

        // The name of the index will be the directory name.
        Name = Path.GetFileName(_indexDirectoryPath);
    }

    ~Index() {
        Dispose(disposing: false);
    }

    /// <inheritdoc />
    public bool IsEmpty() {
        BlockAccessAfterDispose();

        return CountDocuments() <= 0;
    }

    /// <inheritdoc />
    public int CountDocuments() {
        BlockAccessAfterDispose();

        return IndexDirectoryExists() ? GetIndexReader().NumDocs : 0;
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
    public IndexActionResult StoreDocuments(IDocument[] documents) {
        BlockAccessAfterDispose();

        Prevent.Argument.Null(documents);

        if (documents.Length == 0) {
            return IndexActionResult.Success(count: 0);
        }

        var result = InnerDeleteDocuments(documents);
        if (result.Succeeded) {
            result = InnerStoreDocuments(documents);
        }

        InnerCommitChanges();

        return result;
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException">
    /// if <paramref name="documents"/> is <c>null</c>.
    /// </exception>
    public IndexActionResult DeleteDocuments(IDocument[] documents) {
        BlockAccessAfterDispose();

        Prevent.Argument.Null(documents);

        if (documents.Length == 0) {
            return IndexActionResult.Success(count: 0);
        }

        var result = InnerDeleteDocuments(documents);

        InnerCommitChanges();

        return result;
    }

    private void InnerCommitChanges() {
        try {
            var indexWriter = GetIndexWriter();
            indexWriter.Flush(triggerMerge: true, applyAllDeletes: true);
            indexWriter.PrepareCommit();
            indexWriter.Commit();
        } catch (OutOfMemoryException ex) {
            DestroyIndexWriter();
            _logger.IndexWriterOutOfMemoryError(ex);
        } catch (Exception ex) {
            _logger.CommitChangesError(ex);
        }
    }

    /// <inheritdoc />
    public ISearchBuilder CreateSearchBuilder()
        => new SearchBuilder(_analyzer, GetIndexReader());

    /// <inheritdoc />
    public void Dispose() {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private IndexActionResult InnerStoreDocuments(IEnumerable<IDocument> documents) {
        IndexActionResult result;
        var counter = 0;

        try {
            var indexWriter = GetIndexWriter();

            foreach (var document in documents) {
                indexWriter.AddDocument(doc: document.ToLuceneDocument());
                counter++;
            }

            result = IndexActionResult.Success(counter);
        } catch (OutOfMemoryException ex) {
            DestroyIndexWriter();
            _logger.IndexWriterOutOfMemoryError(ex);
            result = IndexActionResult.Failure(counter, ex.Message);
        } catch (Exception ex) {
            _logger.StoreDocumentError(ex);
            result = IndexActionResult.Failure(counter, ex.Message);
        }

        return result;
    }

    private IndexActionResult InnerDeleteDocuments(IReadOnlyCollection<IDocument> documents) {
        var counter = 0;

        var batches = documents.Count / BatchSize;
        if (documents.Count % BatchSize != 0) {
            ++batches;
        }

        for (var batch = 0; batch < batches; batch++) {
            var query = new BooleanQuery();
            try {
                var documentBatch = documents.Skip(batch * BatchSize)
                                             .Take(BatchSize);

                foreach (var document in documentBatch) {
                    var term = new Term(fld: nameof(ISearchHit.DocumentID),
                                        text: document.ID);

                    var termQuery = new TermQuery(term);

                    var booleanClause = new BooleanClause(query: termQuery,
                                                          occur: Occur.SHOULD);

                    query.Add(booleanClause);
                    counter++;
                }

                GetIndexWriter().DeleteDocuments(query);
            } catch (OutOfMemoryException ex) {
                DestroyIndexWriter();
                _logger.IndexWriterOutOfMemoryError(ex);
                return IndexActionResult.Failure(counter, ex.Message);
            } catch (Exception ex) {
                _logger.DeleteDocumentError(ex);
                return IndexActionResult.Failure(counter, ex.Message);
            }
        }

        return IndexActionResult.Success(counter);
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

        return _indexReader ??= DirectoryReader.Open(_fsDirectory);
    }

    private bool IndexDirectoryExists()
        => System.IO.Directory.Exists(path: _indexDirectoryPath);

    private void BlockAccessAfterDispose() {
        if (_disposed) {
            throw new ObjectDisposedException($"Index: {Name}");
        }
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
}
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Microsoft.Extensions.Logging;

namespace Nameless.Lucene.Impl {
    /// <summary>
    /// Default implementation of <see cref="IIndex"/>
    /// </summary>
    public sealed class Index : IIndex, IDisposable {
        #region Private Read-Only Fields

        private readonly Analyzer _analyzer;
        private readonly string _indexDirectoryPath;
        private readonly ILogger _logger;

        #endregion

        #region Private Fields

        private FSDirectory? _directory;
        private IndexReader? _indexReader;
        private IndexWriter? _indexWriter;
        private bool _disposed;

        #endregion

        #region Public Static Read-Only Properties

        /// <summary>
        /// Gets the batch size.
        /// </summary>
        public static int BatchSize => BooleanQuery.MaxClauseCount;

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="Index"/>.
        /// </summary>
        /// <param name="analyzer">The Lucene analyzer.</param>
        /// <param name="indexDirectoryPath">The base path of the Lucene directory.</param>
        /// <param name="logger">The <see cref="ILogger{Index}"/> instance.</param>
        /// <param name="name">The index name.</param>
        public Index(Analyzer analyzer, string indexDirectoryPath, ILogger<Index> logger, string name) {
            _analyzer = Guard.Against.Null(analyzer, nameof(analyzer));
            _indexDirectoryPath = Guard.Against.NullOrWhiteSpace(indexDirectoryPath, nameof(indexDirectoryPath));
            _logger = Guard.Against.Null(logger, nameof(logger));

            Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));

            Initialize();
        }

        #endregion

        #region Destructor

        ~Index() {
            Dispose(disposing: false);
        }

        #endregion

        #region Private Static Methods

        private static void InnerStoreDocuments(IndexWriter writer, IDocument[] documents) {
            foreach (var document in documents) {
                writer.AddDocument(
                    doc: document.ToLuceneDocument()
                );
            }
        }

        private static void InnerDeleteDocuments(IndexWriter writer, IDocument[] documents) {
            // Process documents by batch as there is a max number
            // of terms a query can contain (1024 by default).
            var pageCount = (documents.Length / BatchSize) + 1;
            for (var page = 0; page < pageCount; page++) {
                var query = new BooleanQuery();
                try {
                    var batch = documents
                        .Skip(page * BatchSize)
                        .Take(BatchSize);

                    foreach (var document in batch) {
                        query.Add(new BooleanClause(
                            query: new TermQuery(
                                t: new Term(
                                    fld: nameof(ISearchHit.DocumentID),
                                    text: document.ID
                                )
                            ),
                            occur: Occur.SHOULD
                        ));
                    }
                    writer.DeleteDocuments(query);
                } catch { /* Just skip error */ }
            }
        }

        #endregion

        #region Private Methods

        private void Initialize()
            => _directory = FSDirectory.Open(
                path: new DirectoryInfo(_indexDirectoryPath)
            );

        private bool IndexDirectoryExists()
            => System.IO.Directory.Exists(_indexDirectoryPath);

        /// <summary>
        /// Dispose on error.
        /// </summary>
        private IndexWriter GetIndexWriter()
            => _indexWriter ??= new IndexWriter(_directory, new IndexWriterConfig(Root.Defaults.LuceneVersion, _analyzer));

        private IndexReader GetIndexReader()
            => _indexReader ??= DirectoryReader.Open(_directory);

        private void Dispose(bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                _indexWriter?.Dispose();
                _indexReader?.Dispose();
                _directory?.Dispose();
            }

            _directory = null;
            _indexReader = null;
            _indexWriter = null;

            _disposed = true;
        }

        #endregion

        #region IIndex Members

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public bool IsEmpty()
            => CountDocuments() <= 0;

        /// <inheritdoc />
        public int CountDocuments()
            => IndexDirectoryExists() ? GetIndexReader().NumDocs : 0;

        /// <inheritdoc />
        public IDocument NewDocument(string documentID)
            => new Document(documentID);

        /// <inheritdoc />
        public void StoreDocuments(IDocument[] documents) {
            if (documents.Length == 0) { return; }

            var writer = GetIndexWriter();

            InnerDeleteDocuments(writer, documents);
            InnerStoreDocuments(writer, documents);
            InnerCommitChanges(writer);
        }

        /// <inheritdoc />
        public void DeleteDocuments(IDocument[] documents) {
            if (documents.Length == 0) { return; }

            var writer = GetIndexWriter();

            InnerDeleteDocuments(writer, documents);
            InnerCommitChanges(writer);
        }

        private void InnerCommitChanges(ITwoPhaseCommit writer) {
            try {
                writer.PrepareCommit();
                writer.Commit();

                // unfortunately, given the nature of IndexReader,
                // we need to dispose it after changes in the index,
                // like store or delete documents.
                _indexReader?.Dispose();
                _indexReader = null;
            } catch (OutOfMemoryException ex) {
                _logger.LogError(ex, "Out of memory. Disposing IndexWriter.");
                _indexWriter?.Dispose();
            } catch (Exception ex) {
                _logger.LogError(ex, "Error while flushing and commit Index changes.");
            }
        }

        /// <inheritdoc />
        public ISearchBuilder CreateSearchBuilder()
            => new SearchBuilder(_analyzer, GetIndexReader());

        #endregion

        #region IDisposable Members

        /// <inheritdoc />
        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}

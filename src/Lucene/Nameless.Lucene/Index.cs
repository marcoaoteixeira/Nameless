using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene_Directory = Lucene.Net.Store.Directory;
using Lucene_Document = Lucene.Net.Documents.Document;
using Lucene_FSDirectory = Lucene.Net.Store.FSDirectory;

namespace Nameless.Lucene {

    /// <summary>
    /// Default implementation of <see cref="IIndex"/>
    /// </summary>
    public sealed class Index : IIndex, IDisposable {

        #region Private Constants

        private const string DATE_PATTERN = "yyyy-MM-ddTHH:mm:ssZ";

        #endregion

        #region Private Read-Only Fields

        private readonly Analyzer _analyzer;
        private readonly string _indexDirectoryPath;
        private readonly string _name;

        private readonly object _syncLock = new();

        #endregion

        #region Private Fields

        private Lucene_Directory? _directory;
        private IndexReader? _indexReader;
        private IndexSearcher? _indexSearcher;
        private bool _disposed;

        #endregion

        #region Public Static Read-Only Properties

        /// <summary>
        /// GEts the default minimum date time.
        /// </summary>
        public static DateTime DefaultMinDateTime => new(1980, 1, 1);

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
        /// <param name="name">The index name.</param>
        public Index(Analyzer analyzer, string indexDirectoryPath, string name) {
            Prevent.Null(analyzer, nameof(analyzer));
            Prevent.NullOrWhiteSpaces(indexDirectoryPath, nameof(indexDirectoryPath));
            Prevent.NullOrWhiteSpaces(name, nameof(name));

            _analyzer = analyzer;
            _indexDirectoryPath = indexDirectoryPath;
            _name = name;

            Initialize();
        }

        #endregion

        #region Destructor

        /// <summary>
        /// Destructor
        /// </summary>
        ~Index() {
            Dispose(disposing: false);
        }

        #endregion

        #region Private Static Methods

        private static Lucene_Document CreateDocument(IDocument document) {
            Prevent.Null(document, nameof(document));

            var luceneDocument = new Lucene_Document();
            foreach (var field in document.Fields) {
                if (field.Value == default) { continue; }
                var fieldName = field.Name;
                var fieldValue = field.Value;

                if (fieldValue == default) { continue; }

                var store = field.Options.HasFlag(FieldOptions.Store) ? global::Lucene.Net.Documents.Field.Store.YES : global::Lucene.Net.Documents.Field.Store.NO;
                var analyze = field.Options.HasFlag(FieldOptions.Analyze);
                var sanitize = field.Options.HasFlag(FieldOptions.Sanitize);

                switch (field.Type) {
                    case IndexableType.Integer:
                        luceneDocument.Add(new Int32Field(fieldName, Convert.ToInt32(fieldValue), store));
                        break;

                    case IndexableType.Text:
                        var textValue = (string)fieldValue;
                        if (sanitize) { textValue = textValue.RemoveHtmlTags(); }
                        if (analyze) { luceneDocument.Add(new TextField(fieldName, textValue, store)); }
                        else { luceneDocument.Add(new StringField(fieldName, textValue, store)); }
                        break;

                    case IndexableType.DateTime:
                        string dateValue;
                        if (fieldValue is DateTimeOffset offset) {
                            dateValue = offset.ToUniversalTime().ToString(DATE_PATTERN);
                        } else {
                            dateValue = ((DateTime)fieldValue).ToUniversalTime().ToString(DATE_PATTERN);
                        }
                        luceneDocument.Add(new StringField(fieldName, dateValue, store));

                        break;

                    case IndexableType.Boolean:
                        luceneDocument.Add(new StringField(fieldName, fieldValue.ToString()!.ToLower(), store));
                        break;

                    case IndexableType.Number:
                        luceneDocument.Add(new DoubleField(fieldName, Convert.ToDouble(fieldValue), store));
                        break;

                    default:
                        break;
                }
            }
            return luceneDocument;
        }

        #endregion

        #region Private Methods

        private void Initialize() {
            _directory = Lucene_FSDirectory.Open(new DirectoryInfo(_indexDirectoryPath));
            
            // Creates the index directory
            using (CreateIndexWriter()) { }
        }

        private bool IndexDirectoryExists() => Directory.Exists(_indexDirectoryPath);

        private IndexWriter CreateIndexWriter() => new(_directory, new(IndexProvider.Version, _analyzer));

        private IndexReader CreateIndexReader() {
            lock (_syncLock) {
                return _indexReader ??= DirectoryReader.Open(_directory);
            }
        }

        private IndexSearcher CreateIndexSearcher() {
            lock (_syncLock) {
                return _indexSearcher ??= new(CreateIndexReader());
            }
        }

        private void RenewIndex() {
            lock (_syncLock) {
                if (_indexReader != default) {
                    _indexReader.Dispose();
                    _indexReader = default;
                }

                if (_indexSearcher != default) {
                    _indexSearcher = default;
                }
            }
        }

        private void Dispose(bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                _directory?.Dispose();
                _indexReader?.Dispose();
            }

            _directory = default;
            _indexReader = default;
            _indexSearcher = default;
            _disposed = true;
        }

        #endregion

        #region IIndex Members

        /// <inheritdoc />
        public string Name => _name;

        /// <inheritdoc />
        public bool IsEmpty() => TotalDocuments() <= 0;

        /// <inheritdoc />
        public int TotalDocuments() {
            if (!IndexDirectoryExists()) { return -1; }

            return CreateIndexReader().NumDocs;
        }

        /// <inheritdoc />
        public IDocument NewDocument(string documentID) => new Document(documentID);

        /// <inheritdoc />
        public void StoreDocuments(params IDocument[] documents) {
           if (documents.IsNullOrEmpty()) { return; }

            var ids = documents.Select(_ => _.ID).ToArray();

            DeleteDocuments(ids);

            using var writer = CreateIndexWriter();
            foreach (var document in documents) {
                writer.AddDocument(CreateDocument(document));
            }

            RenewIndex();
        }

        /// <inheritdoc />
        public void DeleteDocuments(params string[] documentIDs) {
            if (documentIDs.IsNullOrEmpty()) { return; }

            using var writer = CreateIndexWriter();
            // Process documents by batch as there is a max number of terms
            // a query can contain (1024 by default).
            var pageCount = (documentIDs.Length / BatchSize) + 1;
            for (var page = 0; page < pageCount; page++) {
                var query = new BooleanQuery();
                try {
                    var batch = documentIDs.Skip(page * BatchSize).Take(BatchSize);
                    foreach (var id in batch) {
                        query.Add(new BooleanClause(new TermQuery(new(nameof(ISearchHit.DocumentID), id.ToString())), Occur.SHOULD));
                    }
                    writer.DeleteDocuments(query);
                } catch { /* Just skip error */ }
            }

            RenewIndex();
        }

        /// <inheritdoc />
        public ISearchBuilder CreateSearchBuilder() => new SearchBuilder(_analyzer, CreateIndexSearcher);

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

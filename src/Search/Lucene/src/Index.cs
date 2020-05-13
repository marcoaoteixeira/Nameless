using System;
using System.IO;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using LuceneDirectory = Lucene.Net.Store.Directory;
using LuceneFSDirectory = Lucene.Net.Store.FSDirectory;

namespace Nameless.Search.Lucene {

    /// <summary>
    /// Default implementation of <see cref="IIndex"/>
    /// </summary>
    public sealed class Index : IIndex, IDisposable {

        #region Private Constants

        private const string DATE_PATTERN = "yyyy-MM-ddTHH:mm:ssZ";

        #endregion Private Constants

        #region Private Read-Only Fields

        private readonly Analyzer _analyzer;
        private readonly string _basePath;
        private readonly string _name;

        private readonly object _syncLock = new object ();

        #endregion Private Read-Only Fields

        #region Private Fields

        private LuceneDirectory _directory;
        private IndexReader _indexReader;
        private IndexSearcher _indexSearcher;
        private bool _disposed;

        #endregion Private Fields

        #region Public Static Read-Only Fields

        /// <summary>
        /// GEts the default minimun date time.
        /// </summary>
        public static readonly DateTime DefaultMinDateTime = new DateTime (1980, 1, 1);

        /// <summary>
        /// Gets the batch size.
        /// </summary>
        public static readonly int BatchSize = BooleanQuery.MaxClauseCount;

        #endregion Public Static Read-Only Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="Index"/>.
        /// </summary>
        /// <param name="analyzer">The Lucene analyzer.</param>
        /// <param name="basePath">The base path of the Lucene directory.</param>
        /// <param name="name">The index name.</param>
        public Index (Analyzer analyzer, string basePath, string name) {
            Prevent.ParameterNull (analyzer, nameof (analyzer));
            Prevent.ParameterNullOrWhiteSpace (basePath, nameof (basePath));
            Prevent.ParameterNullOrWhiteSpace (name, nameof (name));

            _analyzer = analyzer;
            _basePath = basePath;
            _name = name;

            Initialize ();
        }

        #endregion Public Constructors

        #region Destructor

        /// <summary>
        /// Destructor
        /// </summary>
        ~Index () {
            Dispose (disposing: false);
        }

        #endregion Destructor

        #region Private Static Methods

        private static Document CreateDocument (IDocumentIndex documentIndex) {
            var documentIndexImpl = documentIndex as DocumentIndex;
            var document = new Document ();
            foreach (var entry in documentIndexImpl.Entries) {
                if (entry.Value.Value == null) { continue; }
                var fieldName = entry.Key;
                var fieldValue = entry.Value.Value;

                var store = entry.Value.Options.HasFlag (DocumentIndexOptions.Store) ? Field.Store.YES : Field.Store.NO;
                var analyze = entry.Value.Options.HasFlag (DocumentIndexOptions.Analyze);
                var sanitize = entry.Value.Options.HasFlag (DocumentIndexOptions.Sanitize);

                switch (entry.Value.Type) {
                    case DocumentIndex.IndexableType.Integer:
                        document.Add (new Int32Field (fieldName, Convert.ToInt32 (fieldValue), store));
                        break;

                    case DocumentIndex.IndexableType.Text:
                        var textValue = Convert.ToString (fieldValue);
                        if (sanitize) { textValue = textValue.RemoveHtmlTags (); }
                        if (analyze) { document.Add (new TextField (fieldName, textValue, store)); } else { document.Add (new StringField (fieldName, textValue, store)); }
                        break;

                    case DocumentIndex.IndexableType.DateTime:
                        string dateValue;
                        if (fieldValue is DateTimeOffset offset) {
                            dateValue = offset.ToUniversalTime ().ToString (DATE_PATTERN);
                        } else {
                            dateValue = ((DateTime) fieldValue).ToUniversalTime ().ToString (DATE_PATTERN);
                        }
                        document.Add (new StringField (fieldName, dateValue, store));

                        break;

                    case DocumentIndex.IndexableType.Boolean:
                        document.Add (new StringField (fieldName, Convert.ToString (fieldValue).ToLower (), store));
                        break;

                    case DocumentIndex.IndexableType.Number:
                        document.Add (new DoubleField (fieldName, Convert.ToDouble (fieldValue), store));
                        break;

                    default:
                        break;
                }
            }
            return document;
        }

        #endregion Private Static Methods

        #region Private Methods

        private void Initialize () {
            _directory = LuceneFSDirectory.Open (new DirectoryInfo (Path.Combine (_basePath, Name)));

            // Creates the index directory
            using (CreateIndexWriter ()) { }
        }

        private bool IndexDirectoryExists () {
            return Directory.Exists (Path.Combine (_basePath, Name));
        }

        private IndexWriter CreateIndexWriter () {
            return new IndexWriter (_directory, new IndexWriterConfig (IndexProvider.Version, _analyzer));
        }

        private IndexReader CreateIndexReader () {
            lock (_syncLock) {
                return _indexReader ?? (_indexReader = DirectoryReader.Open (_directory));
            }
        }

        private IndexSearcher CreateIndexSearcher () {
            lock (_syncLock) {
                return _indexSearcher ?? (_indexSearcher = new IndexSearcher (CreateIndexReader ()));
            }
        }

        private void RenewIndex () {
            lock (_syncLock) {
                if (_indexReader != null) {
                    _indexReader.Dispose ();
                    _indexReader = null;
                }

                if (_indexSearcher != null) {
                    _indexSearcher = null;
                }
            }
        }

        private void Dispose (bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                if (_directory != null) {
                    _directory.Dispose ();
                }

                if (_indexReader != null) {
                    _indexReader.Dispose ();
                }
            }

            _directory = null;
            _indexReader = null;
            _indexSearcher = null;
            _disposed = true;
        }

        #endregion Private Methods

        #region IIndex Members

        /// <inheritdoc />
        public string Name {
            get { return _name; }
        }

        /// <inheritdoc />
        public bool IsEmpty () {
            return TotalDocuments () <= 0;
        }

        /// <inheritdoc />
        public int TotalDocuments () {
            if (!IndexDirectoryExists ()) { return -1; }

            return CreateIndexReader ().NumDocs;
        }

        /// <inheritdoc />
        public IDocumentIndex NewDocument (string documentID) {
            return new DocumentIndex (documentID);
        }

        /// <inheritdoc />
        public void StoreDocuments (params IDocumentIndex[] documents) {
            if (documents == null) { return; }
            if (documents.Length == 0) { return; }

            DeleteDocuments (documents.OfType<DocumentIndex> ().Select (_ => _.DocumentID).ToArray ());

            using (var writer = CreateIndexWriter ()) {
                foreach (var document in documents) {
                    writer.AddDocument (CreateDocument (document));
                }

                RenewIndex ();
            }
        }

        /// <inheritdoc />
        public void DeleteDocuments (params string[] documentIDs) {
            if (documentIDs == null) { return; }
            if (documentIDs.Length == 0) { return; }

            using (var writer = CreateIndexWriter ()) {
                // Process documents by batch as there is a max number of terms a query can contain (1024 by default).
                var pageCount = (documentIDs.Length / BatchSize) + 1;
                for (var page = 0; page < pageCount; page++) {
                    var query = new BooleanQuery ();
                    try {
                        var batch = documentIDs.Skip (page * BatchSize).Take (BatchSize);
                        foreach (var id in batch) {
                            query.Add (new BooleanClause (new TermQuery (new Term (nameof (ISearchHit.DocumentID), id.ToString ())), Occur.SHOULD));
                        }
                        writer.DeleteDocuments (query);
                    } catch { /* Just skip error */ }
                }

                RenewIndex ();
            }
        }

        /// <inheritdoc />
        public ISearchBuilder CreateSearchBuilder () {
            return new SearchBuilder (_analyzer, CreateIndexSearcher);
        }

        #endregion IIndex Members

        #region IDisposable Members

        /// <inheritdoc />
        public void Dispose () {
            Dispose (disposing: true);
            GC.SuppressFinalize (this);
        }

        #endregion IDisposable Members
    }
}
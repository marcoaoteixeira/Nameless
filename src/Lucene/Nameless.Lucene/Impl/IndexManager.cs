using System.Collections.Concurrent;
using Nameless.Infrastructure;

namespace Nameless.Lucene {
    /// <summary>
    /// Default implementation of <see cref="IIndexManager"/>
    /// </summary>
    public sealed class IndexManager : IIndexManager, IDisposable {
        #region Private Read-Only Fields

        private readonly ConcurrentDictionary<string, IIndex> _cache = new(StringComparer.InvariantCultureIgnoreCase);
        private readonly IApplicationContext _applicationContext;
        private readonly IAnalyzerProvider _analyzerProvider;
        private readonly LuceneOptions _options;

        #endregion

        #region Private Fields

        private bool _disposed;

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="IndexManager"/>.
        /// </summary>
        /// <param name="applicationContext">The application context.</param>
        /// <param name="analyzerProvider">The analyzer provider.</param>
        public IndexManager(IApplicationContext applicationContext, IAnalyzerProvider analyzerProvider)
            : this (applicationContext, analyzerProvider, LuceneOptions.Default) { }

        /// <summary>
        /// Initializes a new instance of <see cref="IndexManager"/>.
        /// </summary>
		/// <param name="applicationContext">The application context.</param>
        /// <param name="analyzerProvider">The analyzer provider.</param>
        /// <param name="options">The settings.</param>
        public IndexManager(IApplicationContext applicationContext, IAnalyzerProvider analyzerProvider, LuceneOptions options) {
            _applicationContext = Guard.Against.Null(applicationContext, nameof(applicationContext));
            _analyzerProvider = Guard.Against.Null(analyzerProvider, nameof(analyzerProvider));
            _options = Guard.Against.Null(options, nameof(options));
        }

        #endregion

        #region Destructor

        ~IndexManager() {
            Dispose(disposing: false);
        }

        #endregion

        #region Private Methods

        private void BlockAccessAfterDispose() {
            if (_disposed) {
                throw new ObjectDisposedException(nameof(IndexManager));
            }
        }

        private void Dispose(bool disposing) {
            if (_disposed) { return; }
            if (disposing) {
                var indexes = _cache.Values.ToArray();
                foreach (var index in indexes) {
                    if (index is IDisposable disposable) {
                        disposable.Dispose();
                    }
                }
                _cache.Clear();
            }

            _disposed = true;
        }

        private string GetIndexDirectoryPath(string indexName)
            => Path.Combine(
                _applicationContext.ApplicationDataFolderPath,
                _options.IndexesRootFolderName,
                indexName
            );

        private Index Create(string indexName)
            => new(
                analyzer: _analyzerProvider.GetAnalyzer(indexName),
                indexDirectoryPath: GetIndexDirectoryPath(indexName),
                name: indexName
            );

        #endregion

        #region IIndexProvider Members

        /// <inheritdoc />
        public void Delete(string indexName) {
            BlockAccessAfterDispose();

            if (_cache.TryRemove(indexName, out var index)) {
                if (index is IDisposable disposable) {
                    disposable.Dispose();
                }

                Directory.Delete(
                    path: GetIndexDirectoryPath(indexName),
                    recursive: true
                );
            }
        }

        /// <inheritdoc />
        public bool Exists(string indexName) {
            BlockAccessAfterDispose();

            return _cache.ContainsKey(indexName);
        }

        /// <inheritdoc />
        public IIndex GetOrCreate(string indexName) {
            BlockAccessAfterDispose();

            return _cache.GetOrAdd(indexName, Create);
        }

        /// <inheritdoc />
        public IEnumerable<string> List() {
            BlockAccessAfterDispose();

            return _cache.Keys;
        }

        #endregion

        #region IDisposable Members

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}

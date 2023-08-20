using Lucene.Net.Util;
using Nameless.Infrastructure;

namespace Nameless.Lucene {
    /// <summary>
    /// Default implementation of <see cref="IIndexProvider"/>
    /// </summary>
    public sealed class IndexProvider : IIndexProvider {
        #region Private Static Read-Only Fields

        private static readonly Dictionary<string, IIndex> Cache = new(StringComparer.InvariantCultureIgnoreCase);
        private static readonly object SyncLock = new();

        #endregion

        #region Private Read-Only Fields

        private readonly IApplicationContext _applicationContext;
        private readonly IAnalyzerProvider _analyzerProvider;
        private readonly LuceneOptions _options;

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="IndexProvider"/>.
        /// </summary>
		/// <param name="applicationContext">The application context.</param>
        /// <param name="analyzerProvider">The analyzer provider.</param>
        /// <param name="options">The settings.</param>
        public IndexProvider(IApplicationContext applicationContext, IAnalyzerProvider analyzerProvider, LuceneOptions? options = null) {
            _applicationContext = Guard.Against.Null(applicationContext, nameof(applicationContext));
            _analyzerProvider = Guard.Against.Null(analyzerProvider, nameof(analyzerProvider));
            _options = options ?? LuceneOptions.Default;
        }

        #endregion

        #region Private Methods

        private string GetIndexDirectoryPath(string indexName)
            => Path.Combine(_applicationContext.BasePath, _options.IndexFolder, indexName);

        private Index InnerCreate(string indexName) => new(
            analyzer: _analyzerProvider.GetAnalyzer(indexName),
            indexDirectoryPath: GetIndexDirectoryPath(indexName),
            name: indexName
        );

        #endregion

        #region IIndexProvider Members

        /// <inheritdoc />
        public void Delete(string indexName) {
            lock (SyncLock) {
                if (!Cache.ContainsKey(indexName)) { return; }
                if (Cache[indexName] is IDisposable disposable) {
                    disposable.Dispose();
                }
                Cache.Remove(indexName);

                Directory.Delete(
                    path: GetIndexDirectoryPath(indexName),
                    recursive: true
                );
            }
        }

        /// <inheritdoc />
        public bool Exists(string indexName) => Cache.ContainsKey(indexName);

        /// <inheritdoc />
        public IIndex GetOrCreate(string indexName) {
            lock (SyncLock) {
                if (!Cache.ContainsKey(indexName)) {
                    Cache.Add(indexName, InnerCreate(indexName));
                }

                return Cache[indexName];
            }
        }

        /// <inheritdoc />
        public IEnumerable<string> List() => Cache.Keys;

        #endregion
    }
}

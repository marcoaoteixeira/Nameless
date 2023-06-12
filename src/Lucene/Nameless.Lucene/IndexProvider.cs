using Lucene.Net.Util;
using Microsoft.Extensions.Options;
using Nameless.Infrastructure;

namespace Nameless.Lucene
{

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

		#region Public Static Read-Only Properties

		/// <summary>
		/// Gets the Lucene version used.
		/// </summary>
		public static LuceneVersion Version => LuceneVersion.LUCENE_48;

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="IndexProvider"/>.
        /// </summary>
		/// <param name="applicationContext">The application context.</param>
        /// <param name="analyzerProvider">The analyzer provider.</param>
        /// <param name="options">The settings.</param>
        public IndexProvider(IApplicationContext applicationContext, IAnalyzerProvider analyzerProvider, IOptions<LuceneOptions> options) {
            Prevent.Null(applicationContext, nameof(applicationContext));
            Prevent.Null(analyzerProvider, nameof(analyzerProvider));

			_applicationContext = applicationContext;
            _analyzerProvider = analyzerProvider;
            _options = options.Value ?? LuceneOptions.Default;
		}

		#endregion

		#region Private Methods

		private string GetIndexDirectoryPath(string indexName) => Path.Combine(_applicationContext.DataDirectoryPath, _options.IndexesDirectoryName, indexName);

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

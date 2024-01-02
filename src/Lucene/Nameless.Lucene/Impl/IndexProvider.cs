﻿using Nameless.Infrastructure;

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
        public IndexProvider(IApplicationContext applicationContext, IAnalyzerProvider analyzerProvider)
            : this (applicationContext, analyzerProvider, LuceneOptions.Default) { }

        /// <summary>
        /// Initializes a new instance of <see cref="IndexProvider"/>.
        /// </summary>
		/// <param name="applicationContext">The application context.</param>
        /// <param name="analyzerProvider">The analyzer provider.</param>
        /// <param name="options">The settings.</param>
        public IndexProvider(IApplicationContext applicationContext, IAnalyzerProvider analyzerProvider, LuceneOptions options) {
            _applicationContext = Guard.Against.Null(applicationContext, nameof(applicationContext));
            _analyzerProvider = Guard.Against.Null(analyzerProvider, nameof(analyzerProvider));
            _options = Guard.Against.Null(options, nameof(options));
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
                if (!Cache.TryGetValue(indexName, out var value)) {
                    return;
                }
                if (value is IDisposable disposable) {
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
                if (!Cache.TryGetValue(indexName, out var value)) {
                    value = InnerCreate(indexName);
                    Cache.Add(indexName, value);
                }

                return value;
            }
        }

        /// <inheritdoc />
        public IEnumerable<string> List() => Cache.Keys;

        #endregion
    }
}

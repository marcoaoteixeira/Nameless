using System;
using System.Collections.Generic;
using System.IO;
using Lucene.Net.Util;

namespace Nameless.Search.Lucene {

    /// <summary>
    /// Default implementation of <see cref="IIndexProvider"/>
    /// </summary>
    public sealed class IndexProvider : IIndexProvider {

        #region Private Static Read-Only Fields

        private static readonly IDictionary<string, IIndex> Cache = new Dictionary<string, IIndex> (StringComparer.InvariantCultureIgnoreCase);
        private static readonly object SyncLock = new object ();

        #endregion Private Static Read-Only Fields

        #region Private Read-Only Fields

        private readonly IAnalyzerProvider _analyzerProvider;
        private readonly SearchSettings _settings;

        #endregion Private Read-Only Fields

        #region Public Static Read-Only Fields

        /// <summary>
        /// Gets the Lucene version used.
        /// </summary>
        public static readonly LuceneVersion Version = LuceneVersion.LUCENE_48;

        #endregion Public Static Read-Only Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="IndexProvider"/>.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="analyzerProvider">The analyzer provider.</param>
        public IndexProvider (IAnalyzerProvider analyzerProvider, SearchSettings settings) {
            Prevent.ParameterNull (analyzerProvider, nameof (analyzerProvider));
            Prevent.ParameterNull (settings, nameof (settings));

            _analyzerProvider = analyzerProvider;
            _settings = settings;
        }

        #endregion Public Constructors

        #region Private Methods

        private IIndex InnerCreate (string indexName) => new Index (_analyzerProvider.GetAnalyzer (indexName), _settings.IndexStorageDirectoryPath, indexName);

        #endregion Private Methods

        #region IIndexProvider Members

        /// <inheritdoc />
        public void Delete (string indexName) {
            lock (SyncLock) {
                if (!Cache.ContainsKey (indexName)) { return; }
                if (Cache[indexName] is IDisposable disposable) {
                    disposable.Dispose ();
                }
                Cache.Remove (indexName);

                Directory.Delete (Path.Combine (_settings.IndexStorageDirectoryPath, indexName), recursive : true);
            }
        }

        /// <inheritdoc />
        public bool Exists (string indexName) => Cache.ContainsKey (indexName);

        /// <inheritdoc />
        public IIndex GetOrCreate (string indexName) {
            lock (SyncLock) {
                if (!Cache.ContainsKey (indexName)) {
                    Cache.Add (indexName, InnerCreate (indexName));
                }

                return Cache[indexName];
            }
        }

        /// <inheritdoc />
        public IEnumerable<string> List () => Cache.Keys;

        #endregion IIndexProvider Members
    }
}
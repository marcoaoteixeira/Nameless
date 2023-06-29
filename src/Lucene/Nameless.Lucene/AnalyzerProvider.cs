using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;

namespace Nameless.Lucene {
    /// <summary>
    /// Default implementation of <see cref="IAnalyzerProvider"/>.
    /// </summary>
    public sealed class AnalyzerProvider : IAnalyzerProvider {
        #region Private Read-Only Fields

        private readonly IAnalyzerSelector[] _selectors;

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="AnalyzerProvider"/>.
        /// </summary>
        /// <param name="selectors">A collection of <see cref="IAnalyzerSelector"/>.</param>
        /// <exception cref="ArgumentNullException">if <paramref name="selectors"/> is <c>null</c>.</exception>
        public AnalyzerProvider(IAnalyzerSelector[] selectors) {
            _selectors = selectors ?? Array.Empty<IAnalyzerSelector>();
        }

        #endregion

        #region ILuceneAnalyzerProvider

        /// <inheritdoc />
        public Analyzer GetAnalyzer(string indexName) {
            var analyzer = _selectors
                .Select(_ => _.GetAnalyzer(indexName))
                .OrderByDescending(_ => _.Priority)
                .FirstOrDefault();

            return analyzer?.Analyzer ?? Constants.DefaultAnalyzer;
        }

        #endregion
    }
}

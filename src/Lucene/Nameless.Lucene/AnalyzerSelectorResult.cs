using Lucene.Net.Analysis;

namespace Nameless.Lucene {
    /// <summary>
    /// Represents a Lucene analyzer selector result.
    /// </summary>
    public sealed class AnalyzerSelectorResult {
        #region Public Properties

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        public int Priority { get; }

        /// <summary>
        /// Gets or sets the analyzer instance.
        /// </summary>
        public Analyzer Analyzer { get; }

        #endregion

        #region Public Constructors

        public AnalyzerSelectorResult(Analyzer analyzer, int priority = 0) {
            Analyzer = Guard.Against.Null(analyzer, nameof(analyzer));
            Priority = priority;
        }

        #endregion
    }
}

using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;

namespace Nameless.Lucene {
    internal static class Constants {

        #region Internal Static Read-Only Properties

        internal static Analyzer DefaultAnalyzer => new StandardAnalyzer(IndexProvider.LuceneVersion);

        #endregion
    }
}
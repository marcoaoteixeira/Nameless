using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Util;

namespace Nameless.Lucene {
    /// <summary>
    /// This class was proposed to be a root point for this assembly.
    /// 
    /// *** DO NOT IMPLEMENT ANYTHING HERE ***
    /// 
    /// But, it's allowed to use it as a repository for all constants or
    /// default values that you'll use throughout this project.
    /// </summary>
    public static class Root {
        #region Public Static Inner Classes

        public static class Defaults {
            #region Public Static Read-Only Fields

            /// <summary>
            /// Gets the Lucene version used.
            /// </summary>
            public static readonly LuceneVersion LuceneVersion = LuceneVersion.LUCENE_48;

            #endregion

            #region Internal Static Read-Only Properties

            internal static Analyzer Analyzer { get; } = new StandardAnalyzer(matchVersion: LuceneVersion);

            #endregion
        }

        #endregion
    }
}

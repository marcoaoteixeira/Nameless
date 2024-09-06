using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Util;

namespace Nameless.Lucene;

/// <summary>
/// The only purpose of this class is to be an "entrypoint" for this
/// assembly.
/// 
/// *** DO NOT IMPLEMENT ANYTHING HERE ***
/// 
/// But, it's OK to use it as a repository for all constants or default
/// values that you'll use throughout this assembly.
/// </summary>
public static class Root {
    public static class Defaults {
        /// <summary>
        /// Gets the Lucene version used.
        /// </summary>
        public static readonly LuceneVersion Version = LuceneVersion.LUCENE_48;

        internal static Analyzer Analyzer { get; } = new StandardAnalyzer(matchVersion: Version);
    }
}
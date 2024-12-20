﻿using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Util;

namespace Nameless.Lucene.Internals;

internal class Defaults {
    internal static readonly LuceneVersion Version = LuceneVersion.LUCENE_48;
    internal static Analyzer Analyzer { get; } = new StandardAnalyzer(matchVersion: Version);
}

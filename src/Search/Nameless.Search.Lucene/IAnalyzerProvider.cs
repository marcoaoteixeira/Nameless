using Lucene.Net.Analysis;

namespace Nameless.Search.Lucene;

/// <summary>
/// Analyzer provider contract.
/// </summary>
public interface IAnalyzerProvider {
    /// <summary>
    /// Retrieves the analyzer given an index
    /// </summary>
    /// <param name="indexName">The index name</param>
    /// <returns>
    /// An <see cref="Analyzer"/> object associated to the index.
    /// </returns>
    Analyzer GetAnalyzer(string indexName);
}
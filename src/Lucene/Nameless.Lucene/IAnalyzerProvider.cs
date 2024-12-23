using Lucene.Net.Analysis;

namespace Nameless.Lucene;

/// <summary>
/// Provides methods to retrieve an <see cref="Analyzer"/>
/// </summary>
public interface IAnalyzerProvider {
    /// <summary>
    /// Retrieves the analyzer given an index
    /// </summary>
    /// <param name="indexName">The index name</param>
    /// <returns>
    /// An instance of <see cref="Analyzer"/> associated to the index
    /// </returns>
    Analyzer GetAnalyzer(string indexName);
}
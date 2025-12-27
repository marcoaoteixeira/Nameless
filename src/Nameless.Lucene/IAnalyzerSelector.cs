using Nameless.Lucene.Results;

namespace Nameless.Lucene;

/// <summary>
///     Defines an analyzer selector.
/// </summary>
public interface IAnalyzerSelector {
    /// <summary>
    ///     Retrieves an analyzer selector result based on the index name.
    /// </summary>
    /// <param name="indexName">
    ///     The index name.
    /// </param>
    /// <returns>
    ///     An <see cref="AnalyzerSelectorResult" /> object.
    /// </returns>
    AnalyzerSelectorResult GetAnalyzer(string indexName);
}
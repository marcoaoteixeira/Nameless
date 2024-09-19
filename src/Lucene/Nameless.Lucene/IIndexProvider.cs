namespace Nameless.Lucene;

/// <summary>
/// Contract for an <see cref="IIndex"/> provider.
/// </summary>
public interface IIndexProvider {
    /// <summary>
    /// Deletes an index, if it exists.
    /// </summary>
    /// <param name="name">Name of the index.</param>
    void DeleteIndex(string name);

    /// <summary>
    /// Checks whether an index exists or not.
    /// </summary>
    /// <param name="name">Name of the index.</param>
    /// <returns>
    /// <c>true</c> if it exists, otherwise; <c>false</c>.
    /// </returns>
    bool IndexExists(string name);

    /// <summary>
    /// Creates an index.
    /// </summary>
    /// <param name="name">Name of the index.</param>
    /// <returns>The index.</returns>
    IIndex CreateIndex(string name);

    /// <summary>
    /// Lists all indexes.
    /// </summary>
    /// <returns>
    /// A collection of index names.
    /// </returns>
    IEnumerable<string> ListIndexes();
}
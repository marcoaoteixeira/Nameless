namespace Nameless.Search;

/// <summary>
///     Contract for an <see cref="IIndex" /> provider.
/// </summary>
public interface IIndexProvider {
    /// <summary>
    ///     Creates a new index.
    /// </summary>
    /// <param name="name">Name of the index.</param>
    /// <returns>The index.</returns>
    IIndex CreateIndex(string name);

    /// <summary>
    ///     Deletes an index.
    /// </summary>
    /// <param name="name">Name of the index.</param>
    /// <returns>if it was deleted <c>true</c>; otherwise <c>false</c>.</returns>
    bool DeleteIndex(string name);

    /// <summary>
    ///     Checks if an index exists or not.
    /// </summary>
    /// <param name="name">Name of the index.</param>
    /// <returns>
    ///     <c>true</c> if it exists, otherwise; <c>false</c>.
    /// </returns>
    bool IndexExists(string name);

    /// <summary>
    ///     Lists all indexes.
    /// </summary>
    /// <returns>
    ///     A collection of index names.
    /// </returns>
    IEnumerable<string> ListIndexes();
}
namespace Nameless.Search;

/// <summary>
///     Represents a manager for indexes.
/// </summary>
public interface IIndexManager {
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
    /// <returns>
    ///     if it was deleted <see langword="true"/>; otherwise,
    ///     <see langword="false"/>.
    /// </returns>
    bool DeleteIndex(string name);

    /// <summary>
    ///     Checks if an index exists or not.
    /// </summary>
    /// <param name="name">Name of the index.</param>
    /// <returns>
    ///     <see langword="true"/> if it exists; otherwise,
    ///     <see langword="false"/>.
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
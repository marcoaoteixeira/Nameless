namespace Nameless.Search;

/// <summary>
///     Represents an index disposal event arguments.
/// </summary>
public sealed class DisposeIndexEventArgs : EventArgs {
    /// <summary>
    ///     Gets the index name.
    /// </summary>
    public string IndexName { get; }

    /// <summary>
    ///     Initializes a new instance of <see cref="DisposeIndexEventArgs" />.
    /// </summary>
    /// <param name="indexName">The index name.</param>
    public DisposeIndexEventArgs(string indexName) {
        IndexName = Guard.Against.NullOrWhiteSpace(indexName);
    }
}

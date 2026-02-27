namespace Nameless.Lucene.Infrastructure;

/// <summary>
///     Represents a provider of <see cref="IIndex" /> instances.
/// </summary>
public interface IIndexProvider : IDisposable {
    /// <summary>
    ///     Retrieves an <see cref="IIndex" /> instance by its name.
    /// </summary>
    /// <param name="indexName">
    ///     The name of the index.
    /// </param>
    /// <returns>
    ///     An instance of <see cref="IIndex" />.
    /// </returns>
    IIndex Get(string? indexName);
}
using System.Diagnostics.CodeAnalysis;
using Lucene.Net.Documents;

namespace Nameless.Lucene.Repository.Mappings;

/// <summary>
///     Maps entities to and from Lucene <see cref="Document"/> instances using
///     registered <see cref="IEntityDescriptor{TEntity}"/> configurations.
/// </summary>
public interface IMapper {
    /// <summary>
    ///     Converts the given entity to a Lucene <see cref="Document"/>.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="entity">The entity to map.</param>
    /// <returns>A Lucene <see cref="Document"/> representing the entity.</returns>
    Document Map<TEntity>(TEntity entity) where TEntity : class;

    /// <summary>
    ///     Converts a Lucene <see cref="Document"/> to an entity instance.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="document">The Lucene document to map.</param>
    /// <returns>A new <typeparamref name="TEntity"/> instance populated from the document.</returns>
    TEntity Map<TEntity>(Document document) where TEntity : class;

    /// <summary>
    ///     Attempts to retrieve the ID <see cref="PropertyDescriptor{TEntity}"/> for the
    ///     given entity type.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="output">
    ///     When this method returns <see langword="true"/>, contains the ID property descriptor;
    ///     otherwise <see langword="null"/>.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if an ID descriptor was found; otherwise <see langword="false"/>.
    /// </returns>
    bool TryGetID<TEntity>([NotNullWhen(returnValue: true)] out PropertyDescriptor<TEntity>? output) where TEntity : class;
}
using Microsoft.EntityFrameworkCore;

namespace Nameless.EntityFrameworkCore.Config;

/// <summary>
///     Current implementation of <see cref="IEntityTypeConfigurator"/>.
/// </summary>
/// <typeparam name="TEntity">
///     Type of the entity.
/// </typeparam>
public sealed class EntityTypeConfigurator<TEntity> : IEntityTypeConfigurator
    where TEntity : class {
    private readonly IEntityTypeConfiguration<TEntity> _entityTypeConfiguration;

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="EntityTypeConfigurator{TEntity}"/> class.
    /// </summary>
    /// <param name="configuration">
    ///     The entity type configuration.
    /// </param>
    public EntityTypeConfigurator(IEntityTypeConfiguration<TEntity> configuration) {
        _entityTypeConfiguration = Throws.When.Null(configuration);
    }

    /// <inheritdoc />
    public void Apply(ModelBuilder builder) {
        builder.ApplyConfiguration(_entityTypeConfiguration);
    }
}
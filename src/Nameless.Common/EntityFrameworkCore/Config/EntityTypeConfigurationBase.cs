using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nameless.EntityFrameworkCore.Entities;

namespace Nameless.EntityFrameworkCore.Config;

/// <summary>
///     Allows configuration for an entity type.
/// </summary>
/// <typeparam name="TEntity">
///     The entity type to be configured.
/// </typeparam>
/// <typeparam name="TID">
///     The type of the entity's ID.
/// </typeparam>
public abstract class EntityTypeConfigurationBase<TEntity, TID> : IEntityTypeConfiguration<TEntity>
    where TEntity : EntityBase<TID>
    where TID : struct, IEquatable<TID> {
    /// <summary>
    ///     Gets the entity's table name.
    /// </summary>
    protected string TableName { get; }

    /// <summary>
    ///     Initializes a new instance of
    ///     <see cref="EntityTypeConfigurationBase{TEntity,TID}"/> class.
    /// </summary>
    /// <param name="tableName">
    ///     The entity's table name.
    /// </param>
    protected EntityTypeConfigurationBase(string tableName) {
        TableName = Throws.When.NullOrWhiteSpace(tableName);
    }

    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<TEntity> builder) {
        builder.ToTable(TableName);

        builder.HasKey(entity => entity.ID);

        builder.Property(entity => entity.EntityState)
               .HasConversion<string>();

        builder.Property(entity => entity.CreationDate)
               .IsRequired(required: true);

        builder.Property(entity => entity.ModificationDate)
               .IsRequired(required: false);

        ConfigureCore(builder);
    }

    /// <summary>
    ///     Configures the entity of type <typeparamref name="TEntity" />.
    /// </summary>
    /// <param name="builder">
    ///     The builder to be used to configure the entity type.
    /// </param>
    protected virtual void ConfigureCore(EntityTypeBuilder<TEntity> builder) { }
}
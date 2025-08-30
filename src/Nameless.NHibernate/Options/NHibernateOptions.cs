using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using NHibernate.Mapping.ByCode.Impl.CustomizersImpl;

namespace Nameless.NHibernate.Options;

/// <summary>
/// NHibernate options
/// </summary>
public sealed class NHibernateOptions {
    private readonly HashSet<Type> _entities = [];
    private readonly HashSet<Type> _mappings = [];

    /// <summary>
    /// Gets the registered entity types.
    /// </summary>
    public Type[] Entities => [.. _entities];

    /// <summary>
    /// Gets the registered mapping types.
    /// </summary>
    public Type[] Mappings => [.. _mappings];

    /// <summary>
    /// Gets or sets schema export settings.
    /// </summary>
    public SchemaExportSettings SchemaExport { get; set; } = new();

    /// <summary>
    /// Gets or sets common settings.
    /// </summary>
    public CommonSettings Common { get; set; } = new();

    /// <summary>
    /// Gets or sets connection settings.
    /// </summary>
    public ConnectionSettings Connection { get; set; } = new();

    /// <summary>
    /// Gets or sets ADO.NET settings.
    /// </summary>
    public AdoNetSettings AdoNet { get; set; } = new();

    /// <summary>
    /// Gets or sets cache settings.
    /// </summary>
    public CacheSettings Cache { get; set; } = new();

    /// <summary>
    /// Gets or sets query settings.
    /// </summary>
    public QuerySettings Query { get; set; } = new();

    /// <summary>
    /// Gets or sets LINQ to HQL settings.
    /// </summary>
    public LinqToHqlSettings LinqToHql { get; set; } = new();

    /// <summary>
    /// Gets or sets Hibernate Mapping Model to Data Definition Language settings.
    /// </summary>
    public HbmToDdlSettings HbmToDdl { get; set; } = new();

    /// <summary>
    /// Gets or sets proxy factory settings.
    /// </summary>
    public ProxyFactorySettings ProxyFactory { get; set; } = new();

    /// <summary>
    /// Gets or sets collection settings.
    /// </summary>
    public CollectionTypeSettings CollectionType { get; set; } = new();

    /// <summary>
    /// Gets or sets transaction settings.
    /// </summary>
    public TransactionSettings Transaction { get; set; } = new();

    /// <summary>
    /// Gets or sets specific database settings.
    /// </summary>
    public SpecificSettings Specific { get; set; } = new();

    /// <summary>
    /// Registers a mapping for a given entity type.
    /// </summary>
    /// <typeparam name="TMapping">Type of the mapping class.</typeparam>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    /// <returns>
    /// Returns the current instance of <see cref="NHibernateOptions"/> to allow for method chaining.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <typeparamref name="TEntity"/> or
    /// <typeparamref name="TMapping"/> is not instantiable.
    /// </exception>
    public NHibernateOptions RegisterMapping<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMapping, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TEntity>()
        where TEntity : class
        where TMapping : PropertyContainerCustomizer<TEntity> {
        if (!typeof(TEntity).CanInstantiate()) {
            throw new InvalidOperationException($"Entity type '{typeof(TEntity).GetPrettyName()}' must be instantiable.");
        }

        if (!typeof(TMapping).CanInstantiate()) {
            throw new InvalidOperationException($"Mapping type '{typeof(TMapping).GetPrettyName()}' must be instantiable.");
        }

        _entities.Add(typeof(TEntity));
        _mappings.Add(typeof(TMapping));

        return this;
    }

    /// <summary>
    /// Converts the current settings and their nested configurations
    /// into a dictionary.
    /// </summary>
    /// <remarks>
    /// This method retrieves all settings properties of the current option
    /// object that derive from  <see cref="SettingsBase"/>. It then collects
    /// their configuration values and combines them into a single dictionary.
    /// The resulting dictionary will be used to configure NHibernate.
    /// </remarks>
    /// <returns>
    /// A <see cref="Dictionary{TKey, TValue}"/> containing configuration keys
    /// and their associated values.
    /// </returns>
    public Dictionary<string, string> ToDictionary() {
        var configs = new List<KeyValuePair<string, string>>();
        var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                  .Where(property => typeof(SettingsBase).IsAssignableFrom(property.PropertyType));

        foreach (var property in properties) {
            if (property.GetValue(this) is not SettingsBase options) {
                continue;
            }

            configs.AddRange(options.GetConfigValues());
        }

        return new Dictionary<string, string>([.. configs]);
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Contracts;
using Nameless.EntityFrameworkCore.Config;
using Nameless.Registration;

namespace Nameless.EntityFrameworkCore;

/// <summary>
///     Configuration object used to register Entity Framework Core interceptors
///     and database seeders.
/// </summary>
public class EntityFrameworkCoreFeatureOptions : AssemblyScanAware<EntityFrameworkCoreFeatureOptions> {
    private readonly List<Type> _interceptors = [];
    private readonly List<Type> _databaseSeeders = [];

    internal Action<IServiceCollection>? EntityTypeConfigurationCallback { get; private set; }

    /// <summary>
    ///     Whether it should register the Database Context Factory instead
    ///     of the Database Context. Useful in situations where the dependency
    ///     injection scope is not aligned with the context lifetime.
    /// </summary>
    public bool UseDbContextFactory { get; private set; }

    /// <summary>
    ///     Gets or sets an optional override for the default
    ///     <see cref="DbContextOptionsBuilder"/> configuration.
    ///     When set, replaces the default Sqlite configuration.
    /// </summary>
    public Action<IServiceProvider, DbContextOptionsBuilder>? DbContextConfiguration { get; private set; }

    /// <summary>
    ///     Gets the registered EF Core interceptor types.
    /// </summary>
    /// <remarks>
    ///     Important: Interceptors are not registered using assembly scan
    ///     feature. The order in which they are registered is crucial for
    ///     EF Core, since it executes each interceptor in the order they
    ///     were configured.
    /// </remarks>
    public IReadOnlyCollection<Type> Interceptors => _interceptors;

    /// <summary>
    ///     Gets the registered database seeder type, if any.
    /// </summary>
    public IReadOnlyCollection<Type> DatabaseSeeders => UseAssemblyScan
        ? ExecuteAssemblyScan(typeof(IDatabaseSeeder))
        : _databaseSeeders;

    /// <summary>
    ///     Sets whether it should configure
    ///     <see cref="IDbContextFactory{TContext}"/> instead of a single
    ///     instance of <see cref="DbContext"/>.
    /// </summary>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <returns>
    ///     The current <see cref="EntityFrameworkCoreFeatureOptions"/> so other
    ///     actions can be chained.
    /// </returns>
    public EntityFrameworkCoreFeatureOptions WithUseDbContextFactory(bool value) {
        UseDbContextFactory = value;

        return this;
    }

    /// <summary>
    ///     Sets the delegate to use when configuring the database context.
    /// </summary>
    /// <remarks>
    ///     Database context has a default configuration action, for SQLite.
    ///     When providing a new value, it overrides the entire configuration
    ///     action.
    /// </remarks>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <returns>
    ///     The current <see cref="EntityFrameworkCoreFeatureOptions"/> so other
    ///     actions can be chained.
    /// </returns>
    public EntityFrameworkCoreFeatureOptions WithDbContextConfiguration(Action<IServiceProvider, DbContextOptionsBuilder> value) {
        DbContextConfiguration = value;

        return this;
    }

    /// <summary>
    ///     Registers an EF Core interceptor by type parameter.
    /// </summary>
    /// <typeparam name="TInterceptor">
    ///     The interceptor type to register.
    /// </typeparam>
    /// <returns>
    ///     The current <see cref="EntityFrameworkCoreFeatureOptions"/> so other
    ///     actions can be chained.
    /// </returns>
    public EntityFrameworkCoreFeatureOptions WithInterceptor<TInterceptor>() where TInterceptor : IInterceptor {
        return WithInterceptor(typeof(TInterceptor));
    }

    /// <summary>
    ///     Registers an EF Core interceptor by its <see cref="Type"/>.
    /// </summary>
    /// <param name="type">
    ///     The concrete interceptor type to register.
    /// </param>
    /// <returns>
    ///     The current <see cref="EntityFrameworkCoreFeatureOptions"/> so other
    ///     actions can be chained.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     if <paramref name="type"/> is not assignable from
    ///     <see cref="IInterceptor"/>, is an open generic type, or is a
    ///     non-concrete type.
    /// </exception>
    public EntityFrameworkCoreFeatureOptions WithInterceptor(Type type) {
        Throws.When.OpenGenericType(type);
        Throws.When.NonConcreteType(type);
        Throws.When.NonAssignableFrom(type, typeof(IInterceptor));

        // Interceptors must be registered as list objects since
        // they need to ensure order of execution.
        if (!_interceptors.Contains(type)) {
            _interceptors.Add(type);
        }

        return this;
    }

    /// <summary>
    ///     Registers a database seeder by type parameter.
    /// </summary>
    /// <typeparam name="TDatabaseSeeder">
    ///     The seeder type to register.
    /// </typeparam>
    /// <returns>
    ///     The current <see cref="EntityFrameworkCoreFeatureOptions"/> so other
    ///     actions can be chained.
    /// </returns>
    public EntityFrameworkCoreFeatureOptions WithDatabaseSeeder<TDatabaseSeeder>() where TDatabaseSeeder : IDatabaseSeeder {
        return WithDatabaseSeeder(typeof(TDatabaseSeeder));
    }

    /// <summary>
    ///     Registers a database seeder by its <see cref="Type"/>.
    /// </summary>
    /// <param name="type">
    ///     The concrete seeder type to register.
    /// </param>
    /// <returns>
    ///     The current <see cref="EntityFrameworkCoreFeatureOptions"/> so other
    ///     actions can be chained.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     if <paramref name="type"/> is not assignable from
    ///     <see cref="IDatabaseSeeder"/>, is an open generic type, or is a
    ///     non-concrete type.
    /// </exception>
    public EntityFrameworkCoreFeatureOptions WithDatabaseSeeder(Type type) {
        Throws.When.OpenGenericType(type);
        Throws.When.NonConcreteType(type);
        Throws.When.NonAssignableFrom(type, typeof(IDatabaseSeeder));

        if (!_databaseSeeders.Contains(type)) {
            _databaseSeeders.Add(type);
        }

        return this;
    }

    /// <summary>
    ///     Registers an <see cref="IEntityTypeConfiguration{TEntity}"/> for
    ///     entity <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">
    ///     Type of the entity.
    /// </typeparam>
    /// <typeparam name="TEntityTypeConfiguration">
    ///     Type of the entity type configuration.
    /// </typeparam>
    /// <returns>
    ///     The current <see cref="EntityFrameworkCoreFeatureOptions"/> so other
    ///     actions can be chained.
    /// </returns>
    public EntityFrameworkCoreFeatureOptions WithEntityTypeConfiguration<TEntity, TEntityTypeConfiguration>() where TEntity : class where TEntityTypeConfiguration : class, IEntityTypeConfiguration<TEntity> {
        EntityTypeConfigurationCallback += services => {
            services.AddSingleton<TEntityTypeConfiguration>();
            services.AddSingleton<IEntityTypeConfigurator>(provider => new EntityTypeConfigurator<TEntity>(
                provider.GetRequiredService<TEntityTypeConfiguration>()
            ));
        };

        return this;
    }
}
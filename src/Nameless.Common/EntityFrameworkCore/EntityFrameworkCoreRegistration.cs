using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Nameless.Registration;

namespace Nameless.EntityFrameworkCore;

/// <summary>
///     Configuration object used to register Entity Framework Core interceptors
///     and database seeders.
/// </summary>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class EntityFrameworkCoreRegistration : AssemblyScanAware<EntityFrameworkCoreRegistration> {
    private readonly HashSet<Type> _interceptors = [];

    /// <summary>
    ///     Gets the registered EF Core interceptor types.
    /// </summary>
    public IReadOnlyCollection<Type> Interceptors => _interceptors;

    /// <summary>
    ///     Gets the registered database seeder type, if any.
    /// </summary>
    public Type? DatabaseSeeder { get; private set; }

    /// <summary>
    ///     Gets or sets an optional override for the default
    ///     <see cref="DbContextOptionsBuilder"/> configuration.
    ///     When set, replaces the default Sqlite configuration.
    /// </summary>
    public Action<IServiceProvider, DbContextOptionsBuilder>? OverrideDbContextConfiguration { get; set; }

    /// <summary>
    ///     Registers an EF Core interceptor by type parameter.
    /// </summary>
    /// <typeparam name="TInterceptor">The interceptor type to register.</typeparam>
    /// <returns>
    ///     The current <see cref="EntityFrameworkCoreRegistration"/> so other actions can be chained.
    /// </returns>
    public EntityFrameworkCoreRegistration RegisterInterceptor<TInterceptor>()
        where TInterceptor : IInterceptor {
        return RegisterInterceptor(typeof(TInterceptor));
    }

    /// <summary>
    ///     Registers an EF Core interceptor by its <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The concrete interceptor type to register.</param>
    /// <returns>
    ///     The current <see cref="EntityFrameworkCoreRegistration"/> so other actions can be chained.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     if <paramref name="type"/> is not assignable from <see cref="IInterceptor"/>,
    ///     is an open generic type, or is a non-concrete type.
    /// </exception>
    public EntityFrameworkCoreRegistration RegisterInterceptor(Type type) {
        Throws.When.IsNotAssignableFrom(type, typeof(IInterceptor));
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNonConcreteType(type);

        _interceptors.Add(type);

        return this;
    }

    /// <summary>
    ///     Registers a database seeder by type parameter.
    /// </summary>
    /// <typeparam name="TDatabaseSeeder">The seeder type to register.</typeparam>
    /// <returns>
    ///     The current <see cref="EntityFrameworkCoreRegistration"/> so other actions can be chained.
    /// </returns>
    public EntityFrameworkCoreRegistration RegisterDatabaseSeeder<TDatabaseSeeder>()
        where TDatabaseSeeder : IDatabaseSeeder {
        return RegisterDatabaseSeeder(typeof(TDatabaseSeeder));
    }

    /// <summary>
    ///     Registers a database seeder by its <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The concrete seeder type to register.</param>
    /// <returns>
    ///     The current <see cref="EntityFrameworkCoreRegistration"/> so other actions can be chained.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     if <paramref name="type"/> is not assignable from <see cref="IDatabaseSeeder"/>,
    ///     is an open generic type, or is a non-concrete type.
    /// </exception>
    public EntityFrameworkCoreRegistration RegisterDatabaseSeeder(Type type) {
        Throws.When.IsNotAssignableFrom(type, typeof(IDatabaseSeeder));
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNonConcreteType(type);

        DatabaseSeeder = type;

        return this;
    }
}
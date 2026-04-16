using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Nameless.Registration;

namespace Nameless.EntityFrameworkCore;

public class EntityFrameworkCoreRegistration : AssemblyScanAware<EntityFrameworkCoreRegistration> {
    private readonly HashSet<Type> _interceptors = [];

    public IReadOnlyCollection<Type> Interceptors => _interceptors;

    public Type? DatabaseSeeder { get; private set; }

    public Action<IServiceProvider, DbContextOptionsBuilder>? OverrideDbContextConfiguration { get; set; }

    public EntityFrameworkCoreRegistration RegisterInterceptor<TInterceptor>()
        where TInterceptor : IInterceptor {
        return RegisterInterceptor(typeof(TInterceptor));
    }

    public EntityFrameworkCoreRegistration RegisterInterceptor(Type type) {
        Throws.When.IsNotAssignableFrom(type, typeof(IInterceptor));
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNonConcreteType(type);

        _interceptors.Add(type);

        return this;
    }

    public EntityFrameworkCoreRegistration RegisterDatabaseSeeder<TDatabaseSeeder>()
        where TDatabaseSeeder : IDatabaseSeeder {
        return RegisterDatabaseSeeder(typeof(TDatabaseSeeder));
    }

    public EntityFrameworkCoreRegistration RegisterDatabaseSeeder(Type type) {
        Throws.When.IsNotAssignableFrom(type, typeof(IDatabaseSeeder));
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNonConcreteType(type);

        DatabaseSeeder = type;

        return this;
    }
}
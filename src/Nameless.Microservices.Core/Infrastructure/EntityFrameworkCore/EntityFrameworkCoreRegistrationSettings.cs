using Microsoft.EntityFrameworkCore.Diagnostics;
using Nameless.Registration;

namespace Nameless.Microservices.Infrastructure.EntityFrameworkCore;

public class EntityFrameworkCoreRegistrationSettings : AssemblyScanAware<EntityFrameworkCoreRegistrationSettings> {
    private readonly HashSet<Type> _interceptors = [];

    public IReadOnlyCollection<Type> Interceptors => UseAssemblyScan
        ? GetImplementationsFor<IInterceptor>(includeGenericDefinition: false)
        : _interceptors;

    public Type? DatabaseSeeder {
        get => UseAssemblyScan
            ? GetImplementationsFor<IDatabaseSeeder>(includeGenericDefinition: false)
                .FirstOrDefault(type => type != typeof(NullDatabaseSeeder))
            : field;
        set;
    }

    public EntityFrameworkCoreRegistrationSettings RegisterInterceptor<TInterceptor>()
        where TInterceptor : IInterceptor {
        return RegisterInterceptor(typeof(TInterceptor));
    }

    public EntityFrameworkCoreRegistrationSettings RegisterInterceptor(Type type) {
        Throws.When.IsNotAssignableFromGeneric(type, typeof(IInterceptor));
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNonConcreteType(type);

        _interceptors.Add(type);

        return this;
    }

    public EntityFrameworkCoreRegistrationSettings SetDatabaseSeeder<TDatabaseSeeder>()
        where TDatabaseSeeder : IDatabaseSeeder {
        return SetDatabaseSeeder(typeof(TDatabaseSeeder));
    }

    public EntityFrameworkCoreRegistrationSettings SetDatabaseSeeder(Type type) {
        Throws.When.IsNotAssignableFromGeneric(type, typeof(IDatabaseSeeder));
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNonConcreteType(type);

        DatabaseSeeder = type;

        return this;
    }
}
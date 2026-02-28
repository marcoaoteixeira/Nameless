using System.Reflection;

namespace Nameless.Registration;

public interface IAssemblyScanAware<out TSelf>
    where TSelf : IAssemblyScanAware<TSelf> {
    bool UseAssemblyScan { get; set; }

    IReadOnlyCollection<Assembly> Assemblies { get; }

    TSelf IncludeAssemblyFrom<TType>();

    TSelf IncludeAssemblies(params IEnumerable<Assembly> assemblies);

    Type DiscoverImplementationFor<TType>(bool includeGenericDefinition);

    Type DiscoverImplementationFor(Type type, bool includeGenericDefinition);

    Type DiscoverImplementationFor<TType>(Type fallback, bool includeGenericDefinition);

    Type DiscoverImplementationFor(Type type, Type fallback, bool includeGenericDefinition);

    IReadOnlyCollection<Type> DiscoverImplementationsFor<TType>(bool includeGenericDefinition);

    IReadOnlyCollection<Type> DiscoverImplementationsFor(Type type, bool includeGenericDefinition);
}
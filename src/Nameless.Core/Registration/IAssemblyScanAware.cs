using System.Reflection;

namespace Nameless.Registration;

public interface IAssemblyScanAware<out TSelf>
    where TSelf : IAssemblyScanAware<TSelf> {
    bool UseAssemblyScan { get; set; }

    IReadOnlyCollection<Assembly> Assemblies { get; }

    TSelf IncludeAssemblyFrom<TType>();

    TSelf IncludeAssemblies(params IEnumerable<Assembly> assemblies);

    IReadOnlyCollection<Type> GetImplementationsFor<TType>(bool includeGenericDefinition);

    IReadOnlyCollection<Type> GetImplementationsFor(Type type, bool includeGenericDefinition);
}
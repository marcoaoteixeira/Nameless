using System.Reflection;

namespace Nameless.Registration;

public abstract class AssemblyScanAware<TSelf> : IAssemblyScanAware<TSelf>
    where TSelf : AssemblyScanAware<TSelf>, new() {
    private readonly HashSet<Assembly> _assemblies = [];

    public bool UseAssemblyScan { get; set; } = true;

    public IReadOnlyCollection<Assembly> Assemblies => _assemblies;

    public TSelf IncludeAssemblyFrom<TType>() {
        return IncludeAssemblies(typeof(TType).Assembly);
    }

    public TSelf IncludeAssemblies(params IEnumerable<Assembly> assemblies) {
        foreach (var assembly in assemblies) {
            _assemblies.Add(assembly);
        }

        return (TSelf)this;
    }

    public IReadOnlyCollection<Type> GetImplementationsFor<TType>(bool includeGenericDefinition) {
        return GetImplementationsFor(typeof(TType), includeGenericDefinition);
    }

    public IReadOnlyCollection<Type> GetImplementationsFor(Type type, bool includeGenericDefinition) {
        var assemblies = Assemblies.Count > 0 ? Assemblies : GetDefaultAssemblies();
        var result = assemblies.GetImplementations(type)
                               .Where(item => includeGenericDefinition
                                   ? includeGenericDefinition
                                   : !item.IsGenericTypeDefinition
                                );

        return [.. result];
    }

    protected virtual Assembly[] GetDefaultAssemblies() {
        return [
            Assembly.GetExecutingAssembly(),
            Assembly.GetCallingAssembly()
        ];
    }
}

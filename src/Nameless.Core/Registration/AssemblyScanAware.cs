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

    public Type DiscoverImplementationFor<TType>(bool includeGenericDefinition = false) {
        var result = DiscoverImplementationsFor<TType>(includeGenericDefinition)
            .SingleOrDefault();

        return result ?? throw new TypeImplementationUnavailableException(typeof(TType));
    }

    public Type DiscoverImplementationFor(Type type, bool includeGenericDefinition = false) {
        var result = DiscoverImplementationsFor(type, includeGenericDefinition: false)
            .SingleOrDefault();

        return result ?? throw new TypeImplementationUnavailableException(type);
    }

    public Type DiscoverImplementationFor<TType>(Type fallback, bool includeGenericDefinition = false) {
        return DiscoverImplementationsFor<TType>(includeGenericDefinition)
            .SingleOrDefault(fallback);
    }

    public Type DiscoverImplementationFor(Type type, Type fallback, bool includeGenericDefinition = false) {
        return DiscoverImplementationsFor(type, includeGenericDefinition)
            .SingleOrDefault(fallback);
    }

    public IReadOnlyCollection<Type> DiscoverImplementationsFor<TType>(bool includeGenericDefinition = false) {
        return DiscoverImplementationsFor(typeof(TType), includeGenericDefinition);
    }

    public IReadOnlyCollection<Type> DiscoverImplementationsFor(Type type, bool includeGenericDefinition = false) {
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

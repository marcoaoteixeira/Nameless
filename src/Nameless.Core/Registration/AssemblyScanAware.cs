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
    
    public IReadOnlyCollection<Type> ExecuteAssemblyScan<TType>(bool includeGenericDefinition = false) {
        return ExecuteAssemblyScan(
            typeof(TType),
            includeGenericDefinition
        );
    }

    public IReadOnlyCollection<Type> ExecuteAssemblyScan(Type type, bool includeGenericDefinition = false) {
        var assemblies = InnerGetAssemblies();
        var result = assemblies.GetImplementations(type)
                               .Where(AssemblyScanAttribute.Include)
                               .Where(item => includeGenericDefinition
                                   ? includeGenericDefinition
                                   : !item.IsGenericTypeDefinition
                                );

        return [.. result];
    }

    protected virtual IReadOnlyCollection<Assembly> InnerGetAssemblies() {
        return Assemblies.Count > 0 ? Assemblies : [
            Assembly.GetExecutingAssembly(),
            Assembly.GetCallingAssembly()
        ];
    }
}

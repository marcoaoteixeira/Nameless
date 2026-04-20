using System.Reflection;

namespace Nameless.Registration;

/// <summary>
///     Base class for automatic registration.
/// </summary>
/// <typeparam name="TSelf">
///     The type of the registration.
/// </typeparam>
public abstract class AssemblyScanAware<TSelf> where TSelf : AssemblyScanAware<TSelf>, new() {
    private readonly HashSet<Assembly> _assemblies = [];

    /// <summary>
    ///     Whether it should use assembly scan to locate implementations.
    /// </summary>
    public bool UseAssemblyScan { get; set; } = true;

    /// <summary>
    ///     Gets the registered assemblies.
    /// </summary>
    public IReadOnlyCollection<Assembly> Assemblies => _assemblies;

    /// <summary>
    ///     Includes the assembly associated to the type.
    /// </summary>
    /// <typeparam name="TType">
    ///     The type.
    /// </typeparam>
    /// <returns>
    ///     The current <see cref="AssemblyScanAware{TSelf}"/> instance.
    /// </returns>
    public TSelf IncludeAssemblyFrom<TType>() {
        return IncludeAssemblies(typeof(TType).Assembly);
    }

    /// <summary>
    ///     Includes the specified assemblies.
    /// </summary>
    /// <param name="assemblies">
    ///     The assemblies.
    /// </param>
    /// <returns>
    ///     The current <see cref="AssemblyScanAware{TSelf}"/> instance.
    /// </returns>
    public TSelf IncludeAssemblies(params IEnumerable<Assembly> assemblies) {
        foreach (var assembly in assemblies) {
            _assemblies.Add(assembly);
        }

        return (TSelf)this;
    }
    
    /// <summary>
    ///     Executes the assembly scan over the included assemblies.
    /// </summary>
    /// <typeparam name="TType">
    ///     The base type to scan for.
    /// </typeparam>
    /// <param name="includeGenericTypeDefinition">
    ///     Whether it should include open generic types in the scan.
    /// </param>
    /// <returns>
    ///     A <see cref="IReadOnlyCollection{T}"/> of types that implements
    ///     the <typeparamref name="TType"/>.
    /// </returns>
    public IReadOnlyCollection<Type> ExecuteAssemblyScan<TType>(bool includeGenericTypeDefinition = false) {
        return ExecuteAssemblyScan(
            typeof(TType),
            includeGenericTypeDefinition
        );
    }

    /// <summary>
    ///     Executes the assembly scan over the included assemblies.
    /// </summary>
    /// <param name="type">
    ///     The base type to scan for.
    /// </param>
    /// <param name="includeGenericTypeDefinition">
    ///     Whether it should include open generic types in the scan.
    /// </param>
    /// <returns>
    ///     A <see cref="IReadOnlyCollection{T}"/> of types that implements
    ///     the <paramref name="type"/>.
    /// </returns>
    public IReadOnlyCollection<Type> ExecuteAssemblyScan(Type type, bool includeGenericTypeDefinition = false) {
        var assemblies = InnerGetAssemblies();
        var result = assemblies.GetImplementations(type)
                               .Where(IgnoreAssemblyScanAttribute.IsNotPresent)
                               .Where(item => includeGenericTypeDefinition ? includeGenericTypeDefinition : !item.IsGenericTypeDefinition);

        return [.. result];
    }

    /// <summary>
    ///     Retrieves the included assemblies; if no assembly was included,
    ///     it returns the executing and calling assembly.
    /// </summary>
    /// <returns>
    ///     The included assemblies to scan for implementations.
    /// </returns>
    protected virtual IReadOnlyCollection<Assembly> InnerGetAssemblies() {
        return Assemblies.Count > 0 ? Assemblies : [
            Assembly.GetExecutingAssembly(),
            Assembly.GetCallingAssembly()
        ];
    }
}

using System.Reflection;

namespace Nameless.Registration;

/// <summary>
///     <see cref="AssemblyScanAware{TSelf}"/> helper.
/// </summary>
public static class AssemblyScanAwareHelper {
    /// <summary>
    ///     Joins a configuration delegate that is applied to an instance
    ///     that extends <see cref="AssemblyScanAware{TSelf}"/> and a list
    ///     of assemblies.
    /// </summary>
    /// <typeparam name="TSelf">
    ///     The registration object type.
    /// </typeparam>
    /// <param name="configuration">
    ///     The configuration action.
    /// </param>
    /// <param name="assemblies">
    ///     The list of assemblies.
    /// </param>
    /// <returns>
    ///     A delegate of the same type that includes the assemblies into
    ///     the registration object.
    /// </returns>
    public static Action<TSelf> Join<TSelf>(Action<TSelf>? configuration, IReadOnlyCollection<Assembly> assemblies)
        where TSelf : AssemblyScanAware<TSelf>, new() {
        return (Action<TSelf>)Delegate.Combine(
            IncludeAssemblies,
            configuration
        );

        void IncludeAssemblies(TSelf opts) {
            opts.IncludeAssemblies(assemblies);
        }
    }
}

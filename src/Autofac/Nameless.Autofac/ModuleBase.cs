using System.Reflection;

namespace Nameless.Autofac;

/// <summary>
/// Base module class for Autofac.
/// </summary>
public abstract class ModuleBase : global::Autofac.Module {
    /// <summary>
    /// Gets the support assemblies.
    /// </summary>
    protected Assembly[] SupportAssemblies { get; }

    /// <summary>
    /// Protected constructor.
    /// </summary>
    protected ModuleBase()
        : this([]) { }

    /// <summary>
    /// Protected constructor.
    /// </summary>
    /// <param name="supportAssemblies">The support assemblies.</param>
    protected ModuleBase(Assembly[] supportAssemblies) {
        SupportAssemblies = Prevent.Argument.Null(supportAssemblies);
    }

    /// <summary>
    /// Retrieves, from support assemblies, a single implementation from the given service type.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <returns>The service implementation <see cref="Type"/>.</returns>
    /// <exception cref="InvalidOperationException">
    /// If more than one implementation were found.
    /// </exception>
    protected Type? SearchForImplementation<TService>()
        => SearchForImplementations(typeof(TService)).SingleOrDefault();

    /// <summary>
    /// Retrieves, from support assemblies, a single implementation from the given service type.
    /// </summary>
    /// <param name="serviceType">The service type.</param>
    /// <returns>The service implementation <see cref="Type"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// If <paramref name="serviceType"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// If more than one implementation were found.
    /// </exception>
    protected Type? SearchForImplementation(Type serviceType)
        => SearchForImplementations(serviceType).SingleOrDefault();

    /// <summary>
    /// Retrieves, from support assemblies, all implementations from the given service type.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <returns>An <see cref="IEnumerable{Type}"/> that contains all possible implementation types.</returns>
    protected IEnumerable<Type> SearchForImplementations<TService>()
        => SearchForImplementations(typeof(TService));

    /// <summary>
    /// Retrieves, from support assemblies, all implementations from the given service type.
    /// </summary>
    /// <param name="serviceType">The service type.</param>
    /// <returns>An <see cref="IEnumerable{Type}"/> that contains all possible implementation types.</returns>
    /// <exception cref="ArgumentNullException">
    /// If <paramref name="serviceType"/> is <c>null</c>.
    /// </exception>
    protected IEnumerable<Type> SearchForImplementations(Type serviceType) {
        Prevent.Argument.Null(serviceType);

        foreach (var assembly in SupportAssemblies) {
            foreach (var service in assembly.SearchForImplementations(serviceType)) {
                yield return service;
            }
        }
    }
}
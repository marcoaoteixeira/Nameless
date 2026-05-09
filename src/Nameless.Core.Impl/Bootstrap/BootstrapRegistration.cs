using Nameless.Registration;

namespace Nameless.Bootstrap;

/// <summary>
///     Configuration object used to register bootstrap steps.
/// </summary>
public class BootstrapRegistration : AssemblyScanAware<BootstrapRegistration> {
    private readonly HashSet<Type> _steps = [];

    /// <summary>
    ///     Gets the collection of registered step types.
    /// </summary>
    public IReadOnlyCollection<Type> Steps => _steps;

    /// <summary>
    ///     Registers a bootstrap step by type parameter.
    /// </summary>
    /// <typeparam name="TStep">The type of the step to register.</typeparam>
    /// <returns>
    ///     The current <see cref="BootstrapRegistration"/> so other actions can be chained.
    /// </returns>
    public BootstrapRegistration RegisterStep<TStep>()
        where TStep : IStep {
        return RegisterStep(typeof(TStep));
    }

    /// <summary>
    ///     Registers a bootstrap step by its <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The concrete step type to register.</param>
    /// <returns>
    ///     The current <see cref="BootstrapRegistration"/> so other actions can be chained.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     if <paramref name="type"/> is not assignable from <see cref="IStep"/>,
    ///     is an open generic type, or is a non-concrete type.
    /// </exception>
    public BootstrapRegistration RegisterStep(Type type) {
        Throws.When.IsNotAssignableFrom(type, typeof(IStep));
        Throws.When.IsOpenGenericType(type);
        Throws.When.IsNonConcreteType(type);

        _steps.Add(type);

        return this;
    }
}

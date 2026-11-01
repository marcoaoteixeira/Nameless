using Nameless.Contracts;
using Nameless.Registration;

namespace Nameless.Windows.Bootstrap;

/// <summary>
///     Configuration object used to register bootstrap steps.
/// </summary>
public class BootstrapperFeatureOptions : AssemblyScanAware<BootstrapperFeatureOptions> {
    private static readonly Type StepType = typeof(IStep);

    private readonly HashSet<Type> _steps = [];

    /// <summary>
    ///     Gets the collection of registered step types.
    /// </summary>
    public IReadOnlyCollection<Type> Steps => UseAssemblyScan
        ? ExecuteAssemblyScan(StepType)
        : _steps;

    /// <summary>
    ///     Registers a bootstrap step by type parameter.
    /// </summary>
    /// <typeparam name="TStep">The type of the step to register.</typeparam>
    /// <returns>
    ///     The current <see cref="BootstrapperFeatureOptions"/> so other actions can be chained.
    /// </returns>
    public BootstrapperFeatureOptions WithStep<TStep>() where TStep : IStep {
        return WithStep(typeof(TStep));
    }

    /// <summary>
    ///     Registers a bootstrap step by its <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The concrete step type to register.</param>
    /// <returns>
    ///     The current <see cref="BootstrapperFeatureOptions"/> so other actions can be chained.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     if <paramref name="type"/> is not assignable from <see cref="IStep"/>,
    ///     is an open generic type, or is a non-concrete type.
    /// </exception>
    public BootstrapperFeatureOptions WithStep(Type type) {
        Throws.When.OpenGenericType(type);
        Throws.When.NonConcreteType(type);
        Throws.When.NonAssignableFrom(type, StepType);

        _steps.Add(type);

        return this;
    }
}

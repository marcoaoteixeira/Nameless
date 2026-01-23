namespace Nameless.Bootstrap;

/// <summary>
///     Provides configuration options for registering and managing bootstrap
///     steps in an application startup sequence.
/// </summary>
public class BootstrapOptions {
    private readonly List<Type> _steps = [];

    /// <summary>
    ///     Gets the collection of step types.
    /// </summary>
    public IReadOnlyCollection<Type> Steps => _steps;

    /// <summary>
    ///     Registers a step of the specified type for inclusion in the
    ///     bootstrap process.
    /// </summary>
    /// <typeparam name="TStep">
    ///     Type of the step.
    /// </typeparam>
    /// <returns>
    ///     The current <see cref="BootstrapOptions"/> instance, so other
    ///     actions can be chained.
    /// </returns>
    public BootstrapOptions RegisterStep<TStep>()
        where TStep : IStep {
        var step = typeof(TStep);

        if (!_steps.Contains(step)) {
            _steps.Add(typeof(TStep));
        }

        return this;
    }
}
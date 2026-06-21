namespace Nameless.Bootstrap.Execution;

/// <summary>
///     Exception thrown when a circular dependency is detected in the bootstrap
///     step dependency graph.
/// </summary>
public class StepCircularReferenceException : Exception {
    /// <summary>
    ///     Gets the name of the step where the circular
    ///     reference was detected.
    /// </summary>
    public string Step { get; }

    /// <summary>
    ///     Initializes a new instance
    ///     of <see cref="StepCircularReferenceException"/> class.
    /// </summary>
    /// <param name="step">The step where the circular reference was detected.</param>
    public StepCircularReferenceException(string step)
        : base($"A recursive dependency was detected in step '{step}'.") {
        Step = step;
    }
}
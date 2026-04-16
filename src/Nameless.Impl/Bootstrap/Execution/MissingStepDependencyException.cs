namespace Nameless.Bootstrap.Execution;

public class MissingStepDependencyException : Exception {
    /// <summary>
    ///     Gets the name of the dependent step.
    /// </summary>
    public string Step { get; }

    /// <summary>
    ///     Gets the name of the missing dependency.
    /// </summary>
    public string Dependency { get; }

    /// <summary>
    ///     Initializes a new instance
    ///     of <see cref="MissingStepDependencyException"/> class.
    /// </summary>
    /// <param name="step">The dependent step.</param>
    /// <param name="dependency">The missing dependency.</param>
    public MissingStepDependencyException(string step, string dependency)
        : base($"Step {step} depends on {dependency} which was not found.") {
        Step = step;
        Dependency = dependency;
    }
}
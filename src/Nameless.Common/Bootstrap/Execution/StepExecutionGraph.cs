using System.Collections;

namespace Nameless.Bootstrap.Execution;

/// <summary>
///     Represents the full dependency-ordered graph of bootstrap step
///     execution levels. Enumerating this object yields levels in the
///     order they must be executed.
/// </summary>
public class StepExecutionGraph : IEnumerable<StepExecutionLevel> {
    private readonly IReadOnlyCollection<StepExecutionLevel> _levels;

    /// <summary>
    ///     Gets the number of execution levels in the graph.
    /// </summary>
    public int LevelCount => _levels.Count;

    /// <summary>
    ///     Gets the total number of steps across all levels.
    /// </summary>
    public int TotalSteps { get; }

    private StepExecutionGraph(IEnumerable<StepExecutionLevel> levels, int totalSteps) {
        _levels = [.. levels];

        TotalSteps = totalSteps;
    }

    /// <summary>
    ///     Creates a new <see cref="StepExecutionGraph"/> instance.
    /// </summary>
    /// <param name="levels">The ordered execution levels.</param>
    /// <param name="totalSteps">The total number of steps.</param>
    /// <returns>A new <see cref="StepExecutionGraph"/> instance.</returns>
    public static StepExecutionGraph Create(IEnumerable<StepExecutionLevel> levels, int totalSteps) {
        return new StepExecutionGraph(levels, totalSteps);
    }

    /// <summary>
    ///     Returns all <see cref="StepExecutionResult"/> instances collected
    ///     from every node across all levels.
    /// </summary>
    /// <returns>
    ///     An <see cref="IEnumerable{T}"/> of <see cref="StepExecutionResult"/>.
    /// </returns>
    public IEnumerable<StepExecutionResult> GetExecutionResults() {
        return from level in _levels from node in level select node.Result;
    }

    /// <inheritdoc />
    public IEnumerator<StepExecutionLevel> GetEnumerator() {
        return _levels.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}
using System.Collections;

namespace Nameless.Bootstrap.Execution;

/// <summary>
///     Represents a single level in the <see cref="StepExecutionGraph"/>.
///     All nodes within a level have their dependencies satisfied and may be
///     executed concurrently.
/// </summary>
public class StepExecutionLevel : IEnumerable<StepExecutionNode> {
    private readonly IReadOnlyCollection<StepExecutionNode> _nodes;

    /// <summary>
    ///     Gets the zero-based level index within the execution graph.
    /// </summary>
    public int Level { get; }

    /// <summary>
    ///     Gets the number of nodes in this level.
    /// </summary>
    public int Count => _nodes.Count;

    private StepExecutionLevel(IEnumerable<StepExecutionNode> nodes, int level) {
        _nodes = [.. nodes];

        Level = level;
    }

    /// <summary>
    ///     Creates a new <see cref="StepExecutionLevel"/> instance.
    /// </summary>
    /// <param name="nodes">The nodes that belong to this level.</param>
    /// <param name="level">The zero-based level index.</param>
    /// <returns>A new <see cref="StepExecutionLevel"/> instance.</returns>
    public static StepExecutionLevel Create(IEnumerable<StepExecutionNode> nodes, int level) {
        return new StepExecutionLevel(nodes, level);
    }

    /// <inheritdoc />
    public IEnumerator<StepExecutionNode> GetEnumerator() {
        return _nodes.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}
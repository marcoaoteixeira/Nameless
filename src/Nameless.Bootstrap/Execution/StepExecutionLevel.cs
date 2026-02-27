using System.Collections;

namespace Nameless.Bootstrap.Execution;

public class StepExecutionLevel : IEnumerable<StepExecutionNode> {
    private readonly IReadOnlyCollection<StepExecutionNode> _nodes;

    public int Level { get; }

    public int Count => _nodes.Count;

    private StepExecutionLevel(IEnumerable<StepExecutionNode> nodes, int level) {
        _nodes = [.. nodes];

        Level = level;
    }

    public static StepExecutionLevel Create(IEnumerable<StepExecutionNode> nodes, int level) {
        return new StepExecutionLevel(nodes, level);
    }

    public IEnumerator<StepExecutionNode> GetEnumerator() {
        return _nodes.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}
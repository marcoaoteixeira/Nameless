using System.Collections;

namespace Nameless.Bootstrap.Execution;

public class StepExecutionGraph : IEnumerable<StepExecutionLevel> {
    private readonly IReadOnlyCollection<StepExecutionLevel> _levels;

    public int Count => _levels.Count;

    public int TotalSteps { get; }

    private StepExecutionGraph(IEnumerable<StepExecutionLevel> levels, int totalSteps) {
        _levels = [.. levels];

        TotalSteps = totalSteps;
    }

    public static StepExecutionGraph Create(IEnumerable<StepExecutionLevel> levels, int totalSteps) {
        return new StepExecutionGraph(levels, totalSteps);
    }

    public IEnumerable<StepExecutionResult> GetExecutionResults() {
        return from level in _levels from node in level select node.Result;
    }

    public IEnumerator<StepExecutionLevel> GetEnumerator() {
        return _levels.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}
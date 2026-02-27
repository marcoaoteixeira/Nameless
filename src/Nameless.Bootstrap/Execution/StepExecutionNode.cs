using System.Diagnostics;

namespace Nameless.Bootstrap.Execution;

[DebuggerDisplay("{DebuggerDisplayValue,nq}")]
public class StepExecutionNode {
    private readonly List<StepExecutionNode> _dependencies = [];
    private readonly List<StepExecutionNode> _dependents = [];

    private string DebuggerDisplayValue => $"Step: {Step.Name}";

    public IStep Step { get; }

    public StepExecutionResult Result { get; }

    public IReadOnlyList<StepExecutionNode> Dependencies => _dependencies;

    public IReadOnlyList<StepExecutionNode> Dependents => _dependents;

    public StepExecutionNode(IStep step) {
        Step = step;
        Result = new StepExecutionResult { StepName = step.Name };
    }

    public void AddDependency(StepExecutionNode node) {
        _dependencies.Add(node);
    }

    public void AddDependent(StepExecutionNode node) {
        _dependents.Add(node);
    }
}
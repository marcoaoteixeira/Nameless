using System.Diagnostics;

namespace Nameless.Bootstrap.Execution;

/// <summary>
///     Represents a single step within a <see cref="StepExecutionGraph"/>,
///     including its dependencies, dependents, and execution result.
/// </summary>
[DebuggerDisplay("{DebuggerDisplayValue,nq}")]
public class StepExecutionNode {
    private readonly List<StepExecutionNode> _dependencies = [];
    private readonly List<StepExecutionNode> _dependents = [];

    private string DebuggerDisplayValue => $"Step: {Step.DisplayName}";

    /// <summary>
    ///     Gets the step associated with this node.
    /// </summary>
    public IStep Step { get; }

    /// <summary>
    ///     Gets the execution result for this node's step.
    /// </summary>
    public StepExecutionResult Result { get; }

    /// <summary>
    ///     Gets the nodes that this node depends on.
    /// </summary>
    public IReadOnlyList<StepExecutionNode> Dependencies => _dependencies;

    /// <summary>
    ///     Gets the nodes that depend on this node.
    /// </summary>
    public IReadOnlyList<StepExecutionNode> Dependents => _dependents;

    /// <summary>
    ///     Initializes a new instance of the <see cref="StepExecutionNode"/> class.
    /// </summary>
    /// <param name="step">The step to wrap.</param>
    public StepExecutionNode(IStep step) {
        Step = step;
        Result = new StepExecutionResult { StepName = step.DisplayName };
    }

    /// <summary>
    ///     Adds a node that this node depends on.
    /// </summary>
    /// <param name="node">The dependency node.</param>
    public void AddDependency(StepExecutionNode node) {
        _dependencies.Add(node);
    }

    /// <summary>
    ///     Adds a node that depends on this node.
    /// </summary>
    /// <param name="node">The dependent node.</param>
    public void AddDependent(StepExecutionNode node) {
        _dependents.Add(node);
    }
}
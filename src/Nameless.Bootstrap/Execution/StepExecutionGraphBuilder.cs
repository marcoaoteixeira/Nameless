namespace Nameless.Bootstrap.Execution;

public static class StepExecutionGraphBuilder {
    public static StepExecutionGraph Create(IEnumerable<IStep> steps) {
        var hash = CreateStepExecutionNodes(steps);

        var currentLevel = 0;
        var levels = new List<StepExecutionLevel>();
        var processed = new HashSet<string>();

        while (processed.Count < hash.Count) {
            var nodes = hash.Values.Where(Filter).ToArray();

            // this should never happen since we validate non-recursive
            // steps before.
            if (nodes.Length == 0) { break; }

            levels.Add(StepExecutionLevel.Create(nodes, currentLevel++));

            foreach (var node in nodes) {
                processed.Add(node.Step.ID);
            }
        }

        return StepExecutionGraph.Create(levels, hash.Count);

        bool Filter(StepExecutionNode node) {
            return !processed.Contains(node.Step.ID)
                   && node.Dependencies.All(dependency
                       => processed.Contains(dependency.Step.ID)
                   );
        }
    }

    private static Dictionary<string, StepExecutionNode> CreateStepExecutionNodes(IEnumerable<IStep> steps) {
        var nodes = new Dictionary<string, StepExecutionNode>();
        var inner = steps.ToArray();

        foreach (var step in inner) {
            nodes[step.ID] = new StepExecutionNode(step);
        }

        // connect all dependencies
        foreach (var step in inner) {
            var node = nodes[step.ID];

            foreach (var dependency in step.Dependencies) {
                if (!nodes.TryGetValue(dependency, out var dependencyNode)) {
                    throw new MissingStepDependencyException(step.ID, dependency);
                }

                node.AddDependency(dependencyNode);
                dependencyNode.AddDependent(node);
            }
        }

        ValidateNonCircularReference(nodes);

        return nodes;
    }

    private static void ValidateNonCircularReference(IDictionary<string, StepExecutionNode> nodes) {
        var visited = new HashSet<string>();
        var recursionStack = new HashSet<string>();

        foreach (var node in nodes.Values) {
            if (HasReference(node, visited, recursionStack)) {
                throw new StepCircularReferenceException(node.Step.ID);
            }
        }
    }

    private static bool HasReference(StepExecutionNode node, HashSet<string> visited, HashSet<string> recursionStack) {
        if (recursionStack.Contains(node.Step.ID)) {
            return true;
        }

        if (!visited.Add(node.Step.ID)) {
            return false;
        }

        recursionStack.Add(node.Step.ID);

        foreach (var dependency in node.Dependencies) {
            if (HasReference(dependency, visited, recursionStack)) {
                return true;
            }
        }

        recursionStack.Remove(node.Step.ID);

        return false;
    }
}
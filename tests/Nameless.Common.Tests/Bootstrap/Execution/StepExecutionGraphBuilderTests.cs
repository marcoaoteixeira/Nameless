using Moq;
using Nameless.Bootstrap.Execution;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Bootstrap.Execution;

[UnitTest]
public class StepExecutionGraphBuilderTests {
    // Creates a Mock<IStep> with the given name and dependencies wired up.
    private static IStep CreateStep(string name, params string[] dependencies) {
        var mock = new Mock<IStep>();
        mock.Setup(s => s.Name).Returns(name);
        mock.Setup(s => s.DisplayName).Returns(name);
        mock.Setup(s => s.Dependencies).Returns(dependencies);
        return mock.Object;
    }

    [Fact]
    public void Create_WithNoSteps_ReturnsEmptyGraph() {
        // arrange
        var steps = Array.Empty<IStep>();

        // act
        var graph = StepExecutionGraphBuilder.Create(steps);

        // assert
        Assert.Equal(0, graph.LevelCount);
        Assert.Equal(0, graph.TotalSteps);
    }

    [Fact]
    public void Create_WithSingleStepNoDependencies_ReturnsSingleLevel() {
        // arrange
        var steps = new[] {
            CreateStep("StepA")
        };

        // act
        var graph = StepExecutionGraphBuilder.Create(steps);

        // assert
        Assert.Multiple(() => {
            Assert.Equal(1, graph.LevelCount);
            Assert.Equal(1, graph.TotalSteps);
        });

        var level = graph.Single();
        Assert.Equal(0, level.Level);
        Assert.Equal(1, level.Count);
        Assert.Equal("StepA", level.Single().Step.Name);
    }

    [Fact]
    public void Create_WithLinearDependencyChain_ReturnsLevelsInDependencyOrder() {
        // arrange
        // StepA -> StepB -> StepC (StepC depends on StepB, StepB depends on StepA)
        var steps = new[] {
            CreateStep("StepA"),
            CreateStep("StepB", "StepA"),
            CreateStep("StepC", "StepB")
        };

        // act
        var graph = StepExecutionGraphBuilder.Create(steps);
        var levels = graph.ToArray();

        // assert
        Assert.Multiple(() => {
            Assert.Equal(3, graph.LevelCount);
            Assert.Equal(3, graph.TotalSteps);
        });

        Assert.Equal("StepA", levels[0].Single().Step.Name);
        Assert.Equal("StepB", levels[1].Single().Step.Name);
        Assert.Equal("StepC", levels[2].Single().Step.Name);
    }

    [Fact]
    public void Create_WithSharedDependency_GroupsIndependentStepsInSameLevel() {
        // arrange
        // StepA has no deps; StepB and StepC both depend on StepA and are independent of each other
        var steps = new[] {
            CreateStep("StepA"),
            CreateStep("StepB", "StepA"),
            CreateStep("StepC", "StepA")
        };

        // act
        var graph = StepExecutionGraphBuilder.Create(steps);
        var levels = graph.ToArray();

        // assert
        Assert.Multiple(() => {
            Assert.Equal(2, graph.LevelCount);
            Assert.Equal(3, graph.TotalSteps);
        });

        // Level 0: StepA
        Assert.Equal(1, levels[0].Count);
        Assert.Equal("StepA", levels[0].Single().Step.Name);

        // Level 1: StepB and StepC can run concurrently
        Assert.Equal(2, levels[1].Count);
        var level1Names = levels[1].Select(n => n.Step.Name).OrderBy(n => n).ToArray();
        Assert.Equal(["StepB", "StepC"], level1Names);
    }

    [Fact]
    public void Create_WithMissingDependency_ThrowsMissingStepDependencyException() {
        // arrange
        // StepB depends on StepA, but StepA is not provided
        var steps = new[] {
            CreateStep("StepB", "StepA")
        };

        // act
        var exception = Assert.Throws<MissingStepDependencyException>(() =>
            StepExecutionGraphBuilder.Create(steps)
        );

        // assert
        Assert.Multiple(() => {
            Assert.Equal("StepB", exception.Step);
            Assert.Equal("StepA", exception.Dependency);
        });
    }

    [Fact]
    public void Create_WithCircularReference_ThrowsStepCircularReferenceException() {
        // arrange
        // StepA depends on StepB, StepB depends on StepA — a direct cycle
        var steps = new[] {
            CreateStep("StepA", "StepB"),
            CreateStep("StepB", "StepA")
        };

        // act & assert
        Assert.Throws<StepCircularReferenceException>(() =>
            StepExecutionGraphBuilder.Create(steps)
        );
    }
}

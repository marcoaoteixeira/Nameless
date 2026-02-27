using Nameless.Bootstrap.Fixtures.Steps;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Bootstrap.Execution;

[UnitTest]
public class StepExecutionGraphBuilderTests {
    [Fact]
    public void WhenCreating_StepsWithDependencies_ThenDependenciesShouldResolveCorrectly() {
        // arrange
        var steps = new IStep[] {
            new Step_Level_1_0(), // 1_0 depends on 1_1, 1_2, 1_3
            new Step_Level_1_1(), // 1_1 depends on 1_2, 1_3
            new Step_Level_1_2(), // 1_2 depends on 1_3
            new Step_Level_1_3(), // 1_3 does not have dependencies

            new Step_Level_2_0(), // 2_0 depends on 2_1, 2_2, 2_3
            new Step_Level_2_1(), // 2_1 does not have dependencies
            new Step_Level_2_2(), // 2_2 depends on 2_1
            new Step_Level_2_3(), // 2_3 depends on 2_2, 2_1
        };

        // act
        var actual = StepExecutionGraphBuilder.Create(steps);

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(actual);

            // In this test case, we have 4 levels of execution
            Assert.Equal(4, actual.Count);

            // With 8 steps in total
            Assert.Equal(8, actual.TotalSteps);

            // First level should be the ones with fewer dependencies
            var first = actual.ElementAt(0);
            Assert.IsType<Step_Level_1_3>(first.First().Step);
            Assert.IsType<Step_Level_2_1>(first.Last().Step);

            var second = actual.ElementAt(1);
            Assert.IsType<Step_Level_1_2>(second.First().Step);
            Assert.IsType<Step_Level_2_2>(second.Last().Step);

            var third = actual.ElementAt(2);
            Assert.IsType<Step_Level_1_1>(third.First().Step);
            Assert.IsType<Step_Level_2_3>(third.Last().Step);

            var forth = actual.ElementAt(3);
            Assert.IsType<Step_Level_1_0>(forth.First().Step);
            Assert.IsType<Step_Level_2_0>(forth.Last().Step);
        });
    }

    [Fact]
    public void WhenCreating_StepsWithDependencies_WhenOneRootStep_WithMultipleChildren_ThenDependenciesShouldResolveCorrectly() {
        // arrange
        var steps = new IStep[] {
            new Step_Child_A(),
            new Step_Root(),
            new Step_Child_C(),
            new Step_Child_B(),
        }; // the order of the array should not influence the builder.

        // act
        var actual = StepExecutionGraphBuilder.Create(steps);

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(actual);

            // In this test case, we have 2 levels of execution (Root and Children)
            Assert.Equal(2, actual.Count);

            // With 4 steps in total
            Assert.Equal(4, actual.TotalSteps);

            // First level should be the children
            var first = actual.ElementAt(0);
            Assert.IsType<Step_Child_A>(first.ElementAt(0).Step);
            Assert.IsType<Step_Child_C>(first.ElementAt(1).Step);
            Assert.IsType<Step_Child_B>(first.ElementAt(2).Step);

            // The root step
            var second = actual.ElementAt(1);
            Assert.IsType<Step_Root>(second.Single().Step);
        });
    }

    [Fact]
    public void WhenCreating_StepsWithMissingDependencies_ThenThrowsException() {
        // arrange
        var steps = new IStep[] {
            new Step_Missing_Dependency()
        };

        // act
        var actual = Record.Exception(() => StepExecutionGraphBuilder.Create(steps));

        // assert
        Assert.IsType<MissingStepDependencyException>(actual);
    }

    [Fact]
    public void WhenCreating_WhenStepsHasCircularReference_ThenThrowsException() {
        // arrange
        var steps = new IStep[] {
            new Step_Circular_X(),
            new Step_Circular_Y()
        };

        // act
        var actual = Record.Exception(() => StepExecutionGraphBuilder.Create(steps));

        // assert
        Assert.IsType<StepCircularReferenceException>(actual);
    }

    [Fact]
    public void WhenCreating_WhenStepsHasSelfReference_ThenThrowsException() {
        // arrange
        var steps = new IStep[] {
            new Step_Self_Dependency()
        };

        // act
        var actual = Record.Exception(() => StepExecutionGraphBuilder.Create(steps));

        // assert
        Assert.IsType<StepCircularReferenceException>(actual);
    }
}

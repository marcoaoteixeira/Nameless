using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Bootstrap.Fixtures.Steps;
using Nameless.Bootstrap.Notification;
using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Mockers.System;

namespace Nameless.Bootstrap;

[UnitTest]
public class BootstrapperTests {
    private static Bootstrapper CreateSut(IEnumerable<IStep> steps) {
        return new Bootstrapper(
            steps,
            NullRetryPolicyFactory.Instance,
            TimeProvider.System,
            NullLogger<Bootstrapper>.Instance
        );
    }

    [Fact]
    public async Task WhenExecutingAsync_WithSteps_ThenExecuteStepsInCorrectOrder() {
        // arrange
        var reports = new List<StepProgress>();
        
        var progress = new ProgressMocker<StepProgress>()
            .WithReport(reports.Add)
            .Build();

        // since Step_Root depends on A, B, C the order will be
        // Step_Child_A
        // Step_Child_B
        // Step_Child_C
        // Step_Root
        var steps = new IStep[] {
            new Step_Root(),
            new Step_Child_A(),
            new Step_Child_B(),
            new Step_Child_C(),
        };

        var sut = CreateSut(steps);

        // act
        await sut.ExecuteAsync(context: [], progress, TestContext.Current.CancellationToken);

        // assert
        Assert.Multiple(() => {
            Assert.NotEmpty(reports);

            var reportSteps = reports.Select(item => item.StepName).Distinct().ToArray();
            var expected = new[] {
                nameof(Step_Child_A),
                nameof(Step_Child_B),
                nameof(Step_Child_C),
                nameof(Step_Root)
            };
            Assert.Equal(expected, reportSteps);
        });
    }
}

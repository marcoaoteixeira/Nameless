using Nameless.Bootstrap;
using Nameless.Bootstrap.Notification;

namespace Nameless;

public class StepBaseTests {
    // --- Name ---

    [Fact]
    public void Name_ReturnsTypeName() {
        // arrange
        var step = new ConcreteStep();

        // assert
        Assert.Equal(nameof(ConcreteStep), step.Name);
    }

    // --- DisplayName ---

    [Fact]
    public void DisplayName_DefaultImplementation_ReturnsSameAsName() {
        // arrange
        var step = new ConcreteStep();

        // assert
        Assert.Equal(step.Name, step.DisplayName);
    }

    // --- IsEnabled ---

    [Fact]
    public void IsEnabled_Default_ReturnsTrue() {
        // arrange
        var step = new ConcreteStep();

        // assert
        Assert.True(step.IsEnabled);
    }

    [Fact]
    public void IsEnabled_WhenConstructedWithFalse_ReturnsFalse() {
        // arrange
        var step = new DisabledStep();

        // assert
        Assert.False(step.IsEnabled);
    }

    // --- IsDisabled (via StepExtensions) ---

    [Fact]
    public void IsDisabled_WhenEnabled_ReturnsFalse() {
        // arrange
        var step = new ConcreteStep();

        // assert
        Assert.False(step.IsDisabled);
    }

    [Fact]
    public void IsDisabled_WhenDisabled_ReturnsTrue() {
        // arrange
        var step = new DisabledStep();

        // assert
        Assert.True(step.IsDisabled);
    }

    // --- Dependencies ---

    [Fact]
    public void Dependencies_Default_ReturnsEmpty() {
        // arrange
        var step = new ConcreteStep();

        // assert
        Assert.Empty(step.Dependencies);
    }

    // --- RetryPolicy ---

    [Fact]
    public void RetryPolicy_Default_ReturnsNull() {
        // arrange
        var step = new ConcreteStep();

        // assert
        Assert.Null(step.RetryPolicy);
    }

    // --- ExecuteAsync ---

    [Fact]
    public async Task ExecuteAsync_WhenCalled_InvokesConcreteImplementation() {
        // arrange
        var ct = TestContext.Current.CancellationToken;
        var step = new ConcreteStep();
        var context = new FlowContext();
        var progress = new NullProgress();

        // act
        await step.ExecuteAsync(context, progress, ct);

        // assert
        Assert.True(step.WasExecuted);
    }

    // --- test doubles ---

    private sealed class ConcreteStep : StepBase {
        public bool WasExecuted { get; private set; }

        public override Task ExecuteAsync(
            FlowContext context,
            IProgress<StepProgress> progress,
            CancellationToken cancellationToken) {
            WasExecuted = true;
            return Task.CompletedTask;
        }
    }

    private sealed class DisabledStep() : StepBase(isEnabled: false) {
        public override Task ExecuteAsync(
            FlowContext context,
            IProgress<StepProgress> progress,
            CancellationToken cancellationToken)
            => Task.CompletedTask;
    }

    private sealed class NullProgress : IProgress<StepProgress> {
        public void Report(StepProgress value) { }
    }
}

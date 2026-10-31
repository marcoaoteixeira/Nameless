using Moq;
using Nameless.Testing.Tools.Attributes;
using Nameless.Windows.Events;
using Nameless.Windows.Localization;
using Nameless.Windows.TaskRunner;
using Nameless.Windows.TaskRunner.UI;

namespace Nameless.Windows.Tests.TaskRunner.UI;

[UnitTest]
public class TaskRunnerWindowViewModelTests {
    private static TaskRunnerWindowViewModel CreateSut() {
        var eventPublisher = new Mock<IEventPublisher>(MockBehavior.Loose);

        return new TaskRunnerWindowViewModel(eventPublisher.Object, NullTranslator.Instance);
    }

    [Fact]
    public async Task ExecuteAsync_DelegateCompletes_SetsCompletionToCompleted() {
        // arrange
        var sut = CreateSut();
        sut.SetHandler(_ => Task.CompletedTask);

        // act
        await sut.ExecuteCommand.ExecuteAsync(null);

        // assert
        Assert.Equal(TaskRunnerOutcome.Completed, await sut.Completion);
        Assert.DoesNotContain(sut.Entries, e => e.Foreground == System.Windows.Media.Brushes.DarkRed);
    }

    [Fact]
    public async Task ExecuteAsync_NoDelegateSet_SetsCompletionToCompleted() {
        // arrange
        var sut = CreateSut();

        // act
        await sut.ExecuteCommand.ExecuteAsync(null);

        // assert
        Assert.Equal(TaskRunnerOutcome.Completed, await sut.Completion);
    }

    [Fact]
    public async Task ExecuteAsync_DelegateThrows_FaultsCompletionWithSameInstance_AddsErrorEntry() {
        // arrange
        var sut = CreateSut();
        var exception = new InvalidOperationException("boom");
        sut.SetHandler(_ => throw exception);

        // act
        await sut.ExecuteCommand.ExecuteAsync(null);

        // assert
        var thrown = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Completion);
        Assert.Same(exception, thrown);
        Assert.Single(sut.Entries, e => e.Foreground == System.Windows.Media.Brushes.DarkRed);
    }

    [Fact]
    public async Task ExecuteAsync_OwnTokenCancelled_SetsCompletionToCancelled_AddsWarningEntryOnly() {
        // arrange
        var sut = CreateSut();
        var started = new TaskCompletionSource();
        sut.SetHandler(async ct => {
            started.TrySetResult();
            await Task.Delay(Timeout.Infinite, ct);
        });

        // act
        var executeTask = sut.ExecuteCommand.ExecuteAsync(null);
        await started.Task;
        await sut.CancelCommand.ExecuteAsync(null);
        await executeTask;

        // assert
        Assert.Equal(TaskRunnerOutcome.Cancelled, await sut.Completion);
        Assert.Single(sut.Entries, e => e.Foreground == System.Windows.Media.Brushes.Goldenrod);
        Assert.DoesNotContain(sut.Entries, e => e.Foreground == System.Windows.Media.Brushes.DarkRed);
    }

    [Fact]
    public async Task ExecuteAsync_ForeignTokenCancellation_FaultsCompletion_AddsErrorEntry() {
        // arrange
        var sut = CreateSut();
        sut.SetHandler(_ => throw new OperationCanceledException("unrelated timeout"));

        // act
        await sut.ExecuteCommand.ExecuteAsync(null);

        // assert
        await Assert.ThrowsAsync<OperationCanceledException>(() => sut.Completion);
        Assert.Single(sut.Entries, e => e.Foreground == System.Windows.Media.Brushes.DarkRed);
    }

    [Fact]
    public async Task CancelAsync_BeforeExecuteEverRan_IsNoOp() {
        // arrange
        var sut = CreateSut();

        // act
        await sut.CancelCommand.ExecuteAsync(null);

        // assert
        Assert.False(sut.Completion.IsCompleted);
    }

    [Fact]
    public async Task CancelAsync_AfterCleanUp_IsNoOp() {
        // arrange
        var sut = CreateSut();
        sut.CleanUp();

        // act & assert (no throw)
        await sut.CancelCommand.ExecuteAsync(null);
        Assert.Equal(TaskRunnerOutcome.Cancelled, await sut.Completion);
    }

    [Fact]
    public async Task CleanUp_BeforeExecute_SetsCompletionToCancelled_ExecuteAfterwardsDoesNotInvokeDelegate() {
        // arrange
        var sut = CreateSut();
        var invoked = false;
        sut.SetHandler(_ => {
            invoked = true;

            return Task.CompletedTask;
        });

        // act
        sut.CleanUp();
        await sut.ExecuteCommand.ExecuteAsync(null);

        // assert
        Assert.Equal(TaskRunnerOutcome.Cancelled, await sut.Completion);
        Assert.False(invoked);
    }

    [Fact]
    public async Task CleanUp_WhileRunning_ResolvesCompletionAsSafetyNet() {
        // arrange
        var sut = CreateSut();
        var started = new TaskCompletionSource();
        sut.SetHandler(async ct => {
            started.TrySetResult();
            await Task.Delay(Timeout.Infinite, ct);
        });

        // act
        var executeTask = sut.ExecuteCommand.ExecuteAsync(null);
        await started.Task;
        sut.CleanUp();

        // assert: Completion resolves promptly, without waiting for the
        // delegate's own unwind through ToggleRunningAsync.
        var completed = await Task.WhenAny(
            sut.Completion,
            Task.Delay(TimeSpan.FromSeconds(2), TestContext.Current.CancellationToken));
        Assert.Same(sut.Completion, completed);
        Assert.Equal(TaskRunnerOutcome.Cancelled, await sut.Completion);

        // the execution eventually unwinds too, without throwing.
        await executeTask;
    }

    [Fact]
    public async Task ExecuteAsync_CloseRequestedDuringStartupDelay_DoesNotInvokeDelegateOrThrow() {
        // arrange
        var sut = CreateSut();
        var invoked = false;
        sut.SetHandler(_ => {
            invoked = true;

            return Task.CompletedTask;
        });

        // act: simulate a title-bar/Alt+F4 close landing during the 200ms
        // ToggleRunningAsync delay, before the delegate ever starts.
        var executeTask = sut.ExecuteCommand.ExecuteAsync(null);
        sut.CleanUp();
        await executeTask;

        // assert
        Assert.False(invoked);
        Assert.Equal(TaskRunnerOutcome.Cancelled, await sut.Completion);
    }

    [Fact]
    public void NotifyCloseBlocked_AddsWarningEntryAtMostOncePerRun() {
        // arrange
        var sut = CreateSut();

        // act
        sut.NotifyCloseBlocked();
        sut.NotifyCloseBlocked();

        // assert
        Assert.Single(sut.Entries, e => e.Foreground == System.Windows.Media.Brushes.Goldenrod);
    }
}

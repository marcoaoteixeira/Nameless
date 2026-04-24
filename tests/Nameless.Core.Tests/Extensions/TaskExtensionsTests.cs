namespace Nameless.Extensions;

public class TaskExtensionsTests {
    // ─── CanContinue ────────────────────────────────────────────────────────

    [Fact]
    public async Task CanContinue_WithCompletedTask_ReturnsTrue() {
        // arrange
        var task = Task.CompletedTask;

        // act & assert
        Assert.True(task.CanContinue());

        await task;
    }

    [Fact]
    public async Task CanContinue_WithFaultedTask_ReturnsFalse() {
        // arrange
        var task = Task.FromException(new InvalidOperationException("fault"));
        try { await task; } catch { /* absorb */ }

        // act & assert
        Assert.False(task.CanContinue());
    }

    [Fact]
    public async Task CanContinue_WithCancelledTask_ReturnsFalse() {
        // arrange
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var task = Task.FromCanceled(cts.Token);
        try { await task; } catch { /* absorb */ }

        // act & assert
        Assert.False(task.CanContinue());
    }

    // ─── SkipContextSync (Task) ─────────────────────────────────────────────

    [Fact]
    public async Task SkipContextSync_Task_CanBeAwaited() {
        // arrange
        var task = Task.CompletedTask;

        // act & assert (no exception thrown)
        await task.SkipContextSync();
    }

    // ─── SkipContextSync (Task<T>) ───────────────────────────────────────────

    [Fact]
    public async Task SkipContextSync_TaskOfT_ReturnsResult() {
        // arrange
        var task = Task.FromResult(42);

        // act
        var result = await task.SkipContextSync();

        // assert
        Assert.Equal(42, result);
    }
}

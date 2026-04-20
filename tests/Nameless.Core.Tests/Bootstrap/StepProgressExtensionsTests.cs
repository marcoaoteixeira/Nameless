using Nameless.Bootstrap.Notification;

namespace Nameless;

public class StepProgressExtensionsTests {
    // ─── ReportInformation ───────────────────────────────────────────────────

    [Fact]
    public void ReportInformation_SetsCorrectTypeAndFields() {
        // arrange
        var progress = new CollectingProgress();

        // act
        progress.ReportInformation("step1", "some message");

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(progress.Last);
            Assert.Equal(StepProgressType.Information, progress.Last!.Type);
            Assert.Equal("step1", progress.Last.StepName);
            Assert.Equal("some message", progress.Last.Message);
        });
    }

    // ─── ReportComplete ──────────────────────────────────────────────────────

    [Fact]
    public void ReportComplete_SetsTypeCompleteAndHundredPercent() {
        // arrange
        var progress = new CollectingProgress();

        // act
        progress.ReportComplete("step1");

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(progress.Last);
            Assert.Equal(StepProgressType.Complete, progress.Last!.Type);
            Assert.Equal(100, progress.Last.PercentageComplete);
        });
    }

    [Fact]
    public void ReportComplete_DefaultMessage_ContainsStepName() {
        // arrange
        var progress = new CollectingProgress();

        // act
        progress.ReportComplete("MyStep");

        // assert
        Assert.Contains("MyStep", progress.Last!.Message);
    }

    [Fact]
    public void ReportComplete_WithCustomMessage_UsesCustomMessage() {
        // arrange
        var progress = new CollectingProgress();

        // act
        progress.ReportComplete("step1", "done!");

        // assert
        Assert.Equal("done!", progress.Last!.Message);
    }

    // ─── ReportFailure ───────────────────────────────────────────────────────

    [Fact]
    public void ReportFailure_WithoutException_SetsTypeFailureAndNoExceptionMetadata() {
        // arrange
        var progress = new CollectingProgress();

        // act
        progress.ReportFailure("step1", "something failed");

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(progress.Last);
            Assert.Equal(StepProgressType.Failure, progress.Last!.Type);
            Assert.True(progress.Last.Metadata is null || !progress.Last.Metadata.ContainsKey("Exception"));
        });
    }

    [Fact]
    public void ReportFailure_WithException_IncludesExceptionAndExceptionTypeInMetadata() {
        // arrange
        var progress = new CollectingProgress();
        var exception = new InvalidOperationException("boom");

        // act
        progress.ReportFailure("step1", "failed", exception);

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(progress.Last?.Metadata);
            Assert.True(progress.Last!.Metadata!.ContainsKey("Exception"));
            Assert.True(progress.Last.Metadata.ContainsKey("ExceptionType"));
            Assert.Equal("boom", progress.Last.Metadata["Exception"]);
            Assert.Equal(nameof(InvalidOperationException), progress.Last.Metadata["ExceptionType"]);
        });
    }

    // ─── ReportRetrying ──────────────────────────────────────────────────────

    [Fact]
    public void ReportRetrying_SetsTypeRetryingAndCorrectMetadata() {
        // arrange
        var progress = new CollectingProgress();
        var delay = TimeSpan.FromSeconds(5);

        // act
        progress.ReportRetrying("step1", attempt: 1, maxAttempts: 3, delay: delay);

        // assert
        Assert.Multiple(() => {
            Assert.NotNull(progress.Last);
            Assert.Equal(StepProgressType.Retrying, progress.Last!.Type);
            Assert.NotNull(progress.Last.Metadata);
            Assert.Equal(1, progress.Last.Metadata!["Attempt"]);
            Assert.Equal(3, progress.Last.Metadata["MaxAttempts"]);
            Assert.Equal(5.0, progress.Last.Metadata["DelaySeconds"]);
        });
    }

    // ─── ReportRunning ───────────────────────────────────────────────────────

    [Fact]
    public void ReportRunning_WithMessage_SetsTypeRunning() {
        // arrange
        var progress = new CollectingProgress();

        // act
        progress.ReportRunning("step1", "running now");

        // assert
        Assert.Equal(StepProgressType.Running, progress.Last!.Type);
    }

    [Fact]
    public void ReportRunning_WithPercentage_SetsPercentageComplete() {
        // arrange
        var progress = new CollectingProgress();

        // act
        progress.ReportRunning("step1", "running", 50);

        // assert
        Assert.Equal(50, progress.Last!.PercentageComplete);
    }

    // ─── ReportStart ─────────────────────────────────────────────────────────

    [Fact]
    public void ReportStart_SetsTypeStartAndZeroPercent() {
        // arrange
        var progress = new CollectingProgress();

        // act
        progress.ReportStart("step1");

        // assert
        Assert.Multiple(() => {
            Assert.Equal(StepProgressType.Start, progress.Last!.Type);
            Assert.Equal(0, progress.Last.PercentageComplete);
        });
    }

    [Fact]
    public void ReportStart_DefaultMessage_ContainsStepName() {
        // arrange
        var progress = new CollectingProgress();

        // act
        progress.ReportStart("MyStep");

        // assert
        Assert.Contains("MyStep", progress.Last!.Message);
    }

    [Fact]
    public void ReportStart_WithCustomMessage_UsesCustomMessage() {
        // arrange
        var progress = new CollectingProgress();

        // act
        progress.ReportStart("step1", "starting!");

        // assert
        Assert.Equal("starting!", progress.Last!.Message);
    }

    // ─── test double ─────────────────────────────────────────────────────────

    private sealed class CollectingProgress : IProgress<StepProgress> {
        public StepProgress? Last { get; private set; }

        public void Report(StepProgress value) {
            Last = value;
        }
    }
}

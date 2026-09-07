using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Mockers.Logging;

namespace Nameless.Resilience;

[UnitTest]
public class RetryPipelineFactoryExtendedTests {
    private static RetryPipelineFactory CreateSut() {
        var logger = new LoggerMocker<RetryPipelineFactory>()
            .WithAnyLogLevel()
            .Build();

        return new RetryPipelineFactory(logger);
    }

    private static RetryPolicyConfiguration BuildConfig(
        int retryCount = 3,
        BackoffType backoff = BackoffType.Constant,
        bool useJitter = false,
        TimeSpan? initialDelay = null,
        Func<Exception, bool>? retryOn = null) {
        return new RetryPolicyConfiguration {
            Tag = "test",
            RetryCount = retryCount,
            InitialDelay = initialDelay ?? TimeSpan.FromMilliseconds(1),
            BackoffType = backoff,
            MaxDelay = TimeSpan.FromMilliseconds(100),
            UseJitter = useJitter,
            RetryOnException = retryOn ?? (_ => true),
            OnRetry = (_, _, _, _) => { }
        };
    }

    [Fact]
    public async Task Create_WithZeroRetries_ReturnsEmptyPipeline() {
        // arrange
        var sut = CreateSut();
        var config = BuildConfig(retryCount: 0);

        // act
        var pipeline = sut.Create(config);

        // assert — a zero-retry pipeline is semantically empty: it executes the delegate once,
        // is not null, and has a tag (RetryPipeline.Empty uses a generated GUID tag each time).
        Assert.NotNull(pipeline);

        // verify the pipeline executes without error (no retries needed)
        var executed = false;
        await pipeline.ExecuteAsync(_ => {
            executed = true;
            return ValueTask.CompletedTask;
        }, CancellationToken.None);

        Assert.True(executed);
    }

    [Fact]
    public async Task Create_WithMaxRetries_PipelineRetriesCorrectly() {
        // arrange
        var sut = CreateSut();
        var config = BuildConfig(retryCount: 3);
        var pipeline = sut.Create(config);

        var attempts = 0;

        // act — the operation fails for the first 3 calls, succeeds on the 4th (= 3 retries + 1 initial)
        var exception = await Record.ExceptionAsync(() =>
            pipeline.ExecuteAsync(_ => {
                attempts++;
                if (attempts < 4) {
                    throw new InvalidOperationException("transient");
                }
                return ValueTask.CompletedTask;
            }, CancellationToken.None).AsTask()
        );

        // assert
        Assert.Null(exception);
        Assert.Equal(4, attempts);
    }

    [Fact]
    public async Task Create_WithExponentialBackoff_PipelineWorks() {
        // arrange
        var sut = CreateSut();
        var config = BuildConfig(retryCount: 2, backoff: BackoffType.Exponential);
        var pipeline = sut.Create(config);
        const string Expected = "ok";

        // act
        var actual = await pipeline.ExecuteAsync(
            _ => ValueTask.FromResult(Expected),
            CancellationToken.None
        );

        // assert
        Assert.Equal(Expected, actual);
    }

    [Fact]
    public async Task Create_WithLinearBackoff_PipelineWorks() {
        // arrange
        var sut = CreateSut();
        var config = BuildConfig(retryCount: 2, backoff: BackoffType.Linear);
        var pipeline = sut.Create(config);
        const string Expected = "linear";

        // act
        var actual = await pipeline.ExecuteAsync(
            _ => ValueTask.FromResult(Expected),
            CancellationToken.None
        );

        // assert
        Assert.Equal(Expected, actual);
    }

    [Fact]
    public async Task Create_WithConstantBackoff_PipelineWorks() {
        // arrange
        var sut = CreateSut();
        var config = BuildConfig(retryCount: 2, backoff: BackoffType.Constant);
        var pipeline = sut.Create(config);

        // act
        var exception = await Record.ExceptionAsync(() =>
            pipeline.ExecuteAsync(_ => ValueTask.CompletedTask, CancellationToken.None).AsTask()
        );

        // assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task Create_WithJitter_PipelineWorks() {
        // arrange
        var sut = CreateSut();
        var config = BuildConfig(retryCount: 2, useJitter: true);
        var pipeline = sut.Create(config);

        // act
        var exception = await Record.ExceptionAsync(() =>
            pipeline.ExecuteAsync(_ => ValueTask.CompletedTask, CancellationToken.None).AsTask()
        );

        // assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task Create_PipelineWithRetries_EventuallySucceeds() {
        // arrange
        var sut = CreateSut();
        var config = BuildConfig(retryCount: 5);
        var pipeline = sut.Create(config);

        var callCount = 0;

        // act — fail twice, then succeed
        var exception = await Record.ExceptionAsync(() =>
            pipeline.ExecuteAsync(_ => {
                callCount++;
                if (callCount <= 2) {
                    throw new InvalidOperationException("will retry");
                }
                return ValueTask.CompletedTask;
            }, CancellationToken.None).AsTask()
        );

        // assert
        Assert.Null(exception);
        Assert.Equal(3, callCount);
    }

    [Fact]
    public async Task Create_WhenOperationCanceled_DoesNotRetry() {
        // arrange
        var sut = CreateSut();
        var config = BuildConfig(retryCount: 5);
        var pipeline = sut.Create(config);

        using var cts = new CancellationTokenSource();

        // act
        var exception = await Record.ExceptionAsync(() =>
            pipeline.ExecuteAsync(_ => {
                cts.Cancel();
                throw new OperationCanceledException(cts.Token);
            }, cts.Token).AsTask()
        );

        // assert — OperationCanceledException must not be swallowed
        Assert.IsAssignableFrom<OperationCanceledException>(exception);
    }

    [Fact]
    public async Task Create_WhenRetryOnExceptionReturnsFalse_DoesNotRetry() {
        // arrange
        var sut = CreateSut();
        var config = BuildConfig(retryCount: 5, retryOn: _ => false);
        var pipeline = sut.Create(config);

        var callCount = 0;

        // act
        var exception = await Record.ExceptionAsync(() =>
            pipeline.ExecuteAsync(_ => {
                callCount++;
                throw new InvalidOperationException("no retry");
            }, CancellationToken.None).AsTask()
        );

        // assert — attempted exactly once; retry predicate returned false
        Assert.NotNull(exception);
        Assert.Equal(1, callCount);
    }

    [Fact]
    public async Task Create_DelayDoesNotExceedMaxDelay() {
        // arrange — exponential with a long initial delay that would exceed MaxDelay
        var sut = CreateSut();
        var config = new RetryPolicyConfiguration {
            Tag = "max-delay-test",
            RetryCount = 3,
            InitialDelay = TimeSpan.FromMilliseconds(500),
            BackoffType = BackoffType.Exponential,
            MaxDelay = TimeSpan.FromMilliseconds(10), // much smaller than exponential would compute
            UseJitter = false,
            RetryOnException = _ => true,
            OnRetry = (_, delay, _, _) => {
                Assert.True(delay <= TimeSpan.FromMilliseconds(10));
            }
        };

        var pipeline = sut.Create(config);
        var attempts = 0;

        // act — let it fail twice then succeed on the 3rd attempt
        await pipeline.ExecuteAsync(_ => {
            attempts++;
            if (attempts < 3) { throw new InvalidOperationException("retry me"); }
            return ValueTask.CompletedTask;
        }, CancellationToken.None);
    }
}

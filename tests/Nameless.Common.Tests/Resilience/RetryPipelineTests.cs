using Polly;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Resilience;

public class RetryPipelineTests {
    [Fact]
    [UnitTest]
    public void Empty_ReturnsNonNullPipeline() {
        // act
        var actual = RetryPipeline.Empty;

        // assert
        Assert.NotNull(actual);
    }

    [Fact]
    [UnitTest]
    public async Task ExecuteAsync_ExecutesDelegate_ReturnsResult() {
        // arrange
        var sut = RetryPipeline.Empty;
        const int Expected = 42;

        // act
        var actual = await sut.ExecuteAsync(
            _ => ValueTask.FromResult(Expected),
            CancellationToken.None
        );

        // assert
        Assert.Equal(Expected, actual);
    }

    [Fact]
    [UnitTest]
    public async Task ExecuteAsync_DelegateThrowsThenSucceeds_RetriesAndReturns() {
        // arrange
        var attempts = 0;
        var configuration = new RetryPolicyConfiguration {
            Tag = "retry-test",
            RetryCount = 1,
            InitialDelay = TimeSpan.Zero,
            BackoffType = BackoffType.Constant,
            MaxDelay = TimeSpan.FromSeconds(1),
            UseJitter = false,
            RetryOnException = _ => true,
            OnRetry = (_, _, _, _) => { }
        };

        var strategy = new ResiliencePipelineBuilder()
            .AddRetry(new Polly.Retry.RetryStrategyOptions {
                MaxRetryAttempts = 1,
                ShouldHandle = new PredicateBuilder().Handle<InvalidOperationException>(),
                DelayGenerator = _ => ValueTask.FromResult<TimeSpan?>(TimeSpan.Zero),
                OnRetry = _ => ValueTask.CompletedTask
            })
            .Build();

        var sut = new RetryPipeline(configuration.Tag, strategy);

        // act
        var result = await sut.ExecuteAsync(ct => {
            attempts++;

            if (attempts == 1) {
                throw new InvalidOperationException("First attempt fails");
            }

            return ValueTask.FromResult(attempts);
        }, CancellationToken.None);

        // assert
        Assert.Equal(2, result);
        Assert.Equal(2, attempts);
    }

    [Fact]
    [UnitTest]
    public void Tag_IsSetCorrectly() {
        // arrange
        const string ExpectedTag = "my-pipeline-tag";
        var sut = new RetryPipeline(ExpectedTag, ResiliencePipeline.Empty);

        // act
        var actual = sut.Tag;

        // assert
        Assert.Equal(ExpectedTag, actual);
    }
}

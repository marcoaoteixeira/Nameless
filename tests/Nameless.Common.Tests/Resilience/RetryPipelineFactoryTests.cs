using Nameless.Testing.Tools.Attributes;
using Nameless.Testing.Tools.Mockers.Logging;

namespace Nameless.Resilience;

public class RetryPipelineFactoryTests {
    private static RetryPipelineFactory CreateSut() {
        var logger = new LoggerMocker<RetryPipelineFactory>()
            .WithAnyLogLevel()
            .Build();

        return new RetryPipelineFactory(logger);
    }

    private static RetryPolicyConfiguration CreateConfiguration(int retryCount = 2) {
        return new RetryPolicyConfiguration {
            Tag = "factory-test",
            RetryCount = retryCount,
            InitialDelay = TimeSpan.FromMilliseconds(10),
            BackoffType = BackoffType.Constant,
            MaxDelay = TimeSpan.FromSeconds(1),
            UseJitter = false,
            RetryOnException = _ => true,
            OnRetry = (_, _, _, _) => { }
        };
    }

    [Fact]
    [UnitTest]
    public void Create_WithValidConfiguration_ReturnsNonNullPipeline() {
        // arrange
        var sut = CreateSut();
        var configuration = CreateConfiguration();

        // act
        var actual = sut.Create(configuration);

        // assert
        Assert.NotNull(actual);
    }

    [Fact]
    [UnitTest]
    public async Task Create_Pipeline_CanExecuteDelegate() {
        // arrange
        var sut = CreateSut();
        var configuration = CreateConfiguration();
        var pipeline = sut.Create(configuration);
        const string Expected = "executed";

        // act
        var actual = await pipeline.ExecuteAsync(
            _ => ValueTask.FromResult(Expected),
            CancellationToken.None
        );

        // assert
        Assert.Equal(Expected, actual);
    }
}

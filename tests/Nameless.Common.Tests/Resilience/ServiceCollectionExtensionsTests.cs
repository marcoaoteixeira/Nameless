using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Resilience;

[UnitTest]
public class ServiceCollectionExtensionsTests {
    [Fact]
    public void RegisterResilience_RegistersRetryPipelineFactory() {
        // arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // act
        var returned = services.RegisterResilience();
        using var provider = services.BuildServiceProvider();

        // assert
        Assert.Multiple(
            () => Assert.Same(services, returned),
            () => Assert.IsType<RetryPipelineFactory>(provider.GetRequiredService<IRetryPipelineFactory>())
        );
    }

    [Fact]
    public void RegisterResilience_CalledTwice_KeepsSingleRegistration() {
        // arrange
        var services = new ServiceCollection();

        // act
        services.RegisterResilience().RegisterResilience();

        // assert
        Assert.Single(services, d => d.ServiceType == typeof(IRetryPipelineFactory));
    }
}

[UnitTest]
public class NullRetryPipelineFactoryTests {
    [Fact]
    public void Instance_IsSingleton() {
        // act & assert
        Assert.Same(NullRetryPipelineFactory.Instance, NullRetryPipelineFactory.Instance);
    }

    [Fact]
    public async Task Create_ReturnsPipelineThatExecutesWorkOnce() {
        // arrange
        var pipeline = NullRetryPipelineFactory.Instance.Create(new RetryPolicyConfiguration {
            Tag = "null", RetryCount = 3, InitialDelay = TimeSpan.Zero, BackoffType = BackoffType.Constant,
            MaxDelay = TimeSpan.Zero, UseJitter = false, RetryOnException = _ => true, OnRetry = (_, _, _, _) => { }
        });
        var calls = 0;

        // act
        await pipeline.ExecuteAsync(_ => { calls++; return ValueTask.CompletedTask; }, TestContext.Current.CancellationToken);

        // assert
        Assert.Equal(1, calls);
    }

    [Fact]
    public async Task Create_ReturnsPipelineThatDoesNotRetryFailures() {
        // arrange
        var pipeline = NullRetryPipelineFactory.Instance.Create(new RetryPolicyConfiguration {
            Tag = "null", RetryCount = 3, InitialDelay = TimeSpan.Zero, BackoffType = BackoffType.Constant,
            MaxDelay = TimeSpan.Zero, UseJitter = false, RetryOnException = _ => true, OnRetry = (_, _, _, _) => { }
        });
        var calls = 0;

        // act
        await Assert.ThrowsAsync<InvalidOperationException>(() => pipeline.ExecuteAsync(
            _ => { calls++; throw new InvalidOperationException(); }, TestContext.Current.CancellationToken).AsTask());

        // assert
        Assert.Equal(1, calls);
    }
}

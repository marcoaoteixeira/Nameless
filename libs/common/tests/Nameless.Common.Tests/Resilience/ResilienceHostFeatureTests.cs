using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Nameless.Hosting;
using Nameless.Testing.Tools.Mockers.Hosting;

namespace Nameless.Resilience;

[UnitTest]
public class ResilienceHostFeatureTests {
    private static ResilienceHostFeature CreateSut(IHostApplicationBuilder builder)
    {
        var feature = new ResilienceHostFeature();
            
        feature.Register(
            builder,
            new HostFeatureContext([], NullLogger.Instance)
        );

        return feature;
    }

    [Fact]
    public void RegisterResilience_RegistersRetryPipelineFactory() {
        // arrange
        var builder = new HostApplicationBuilderMocker().WithServices(services => services.AddLogging())
                                                        .Build();
        
        // act
        var sut = new ResilienceHostFeature();

        sut.Register(
            builder,
            new HostFeatureContext([], NullLogger.Instance)
        );

        using var provider = builder.Services.BuildServiceProvider();

        // assert
        Assert.IsType<RetryPipelineFactory>(provider.GetRequiredService<IRetryPipelineFactory>());
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

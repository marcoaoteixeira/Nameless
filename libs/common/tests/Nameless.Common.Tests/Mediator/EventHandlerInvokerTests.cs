using Microsoft.Extensions.DependencyInjection;
using Nameless.Mediator.Events;

namespace Nameless.Mediator;

internal record FanEvent : IEvent;

internal sealed class FanLoggingHandler(RequestLog log) : IEventHandler<FanEvent> {
    public Task HandleAsync(FanEvent evt, CancellationToken cancellationToken) {
        log.Add("logged");

        return Task.CompletedTask;
    }
}

internal sealed class FanSecondLoggingHandler(RequestLog log) : IEventHandler<FanEvent> {
    public Task HandleAsync(FanEvent evt, CancellationToken cancellationToken) {
        log.Add("second");

        return Task.CompletedTask;
    }
}

// Throws before returning a Task.
internal sealed class FanSyncThrowingHandler : IEventHandler<FanEvent> {
    public Task HandleAsync(FanEvent evt, CancellationToken cancellationToken) {
        throw new InvalidOperationException("sync");
    }
}

internal sealed class FanAsyncThrowingHandler : IEventHandler<FanEvent> {
    public async Task HandleAsync(FanEvent evt, CancellationToken cancellationToken) {
        await Task.Yield();

        throw new InvalidOperationException("async");
    }
}

[UnitTest]
public class EventHandlerInvokerTests {
    private static ServiceProvider BuildProvider(params Type[] handlers) {
        var services = new ServiceCollection();
        services.AddSingleton<RequestLog>();

        foreach (var handler in handlers) {
            services.AddTransient(typeof(IEventHandler<FanEvent>), handler);
        }

        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task PublishAsync_WithoutHandlers_Completes() {
        // arrange
        using var provider = BuildProvider();
        var sut = new EventHandlerInvoker(provider);

        // act
        var exception = await Record.ExceptionAsync(() => sut.PublishAsync(new FanEvent(), TestContext.Current.CancellationToken));

        // assert
        Assert.Null(exception);
    }

    [Fact]
    public async Task PublishAsync_WithSeveralHandlers_RunsAll() {
        // arrange
        using var provider = BuildProvider(typeof(FanLoggingHandler), typeof(FanSecondLoggingHandler));
        var sut = new EventHandlerInvoker(provider);

        // act
        await sut.PublishAsync(new FanEvent(), TestContext.Current.CancellationToken);

        // assert
        Assert.Equivalent(new[] { "logged", "second" }, provider.GetRequiredService<RequestLog>().Entries, strict: true);
    }

    [Fact]
    public async Task PublishAsync_HandlerThrowsSynchronously_StillRunsTheOthersAndRethrowsTheException() {
        // arrange
        using var provider = BuildProvider(typeof(FanSyncThrowingHandler), typeof(FanLoggingHandler));
        var sut = new EventHandlerInvoker(provider);

        // act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.PublishAsync(new FanEvent(), TestContext.Current.CancellationToken));

        // assert
        Assert.Multiple(
            () => Assert.Equal("sync", exception.Message),
            () => Assert.Equal(["logged"], provider.GetRequiredService<RequestLog>().Entries)
        );
    }

    [Fact]
    public async Task PublishAsync_SeveralHandlersThrow_ThrowsAggregateExceptionWithAllOfThem() {
        // arrange
        using var provider = BuildProvider(typeof(FanSyncThrowingHandler), typeof(FanAsyncThrowingHandler), typeof(FanLoggingHandler));
        var sut = new EventHandlerInvoker(provider);

        // act
        var exception = await Assert.ThrowsAsync<AggregateException>(
            () => sut.PublishAsync(new FanEvent(), TestContext.Current.CancellationToken));

        // assert
        Assert.Multiple(
            () => Assert.Equivalent(new[] { "sync", "async" }, exception.InnerExceptions.Select(inner => inner.Message), strict: true),
            () => Assert.Equal(["logged"], provider.GetRequiredService<RequestLog>().Entries)
        );
    }

    [Fact]
    public async Task PublishAsync_WithNullEvent_ThrowsArgumentNullException() {
        // arrange
        using var provider = BuildProvider();
        var sut = new EventHandlerInvoker(provider);

        // act & assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => sut.PublishAsync<FanEvent>(null!, TestContext.Current.CancellationToken));
    }
}

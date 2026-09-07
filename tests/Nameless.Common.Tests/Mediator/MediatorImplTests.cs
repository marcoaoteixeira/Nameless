using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Nameless.Mediator.Events;
using Nameless.Mediator.Requests;
using Nameless.Mediator.Streams;
using Nameless.Testing.Tools.Attributes;

namespace Nameless.Mediator;

[UnitTest]
public class MediatorImplTests {
    // Builds a service provider with mediator infrastructure registered manually,
    // bypassing the MediatorRegistration helper whose GetInterfacesThatCloses /
    // FixTypeReference logic relies on FullName being non-null — which breaks when
    // handler types live in a different assembly from their handler interfaces.
    private static IServiceProvider BuildProvider(Action<IServiceCollection> configure) {
        var services = new ServiceCollection();
        services.AddTransient<IEventHandlerInvoker, EventHandlerInvoker>();
        services.AddTransient<IRequestHandlerInvoker, RequestHandlerInvoker>();
        services.AddTransient<IStreamHandlerInvoker, StreamHandlerInvoker>();
        services.AddTransient<IMediator, MediatorImpl>();
        configure(services);
        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task ExecuteAsync_WithRegisteredHandler_ReturnsHandlerResult() {
        // arrange
        var provider = BuildProvider(s =>
            s.AddTransient<IRequestHandler<MediatorTestRequest, string>, MediatorTestRequestHandler>()
        );
        var mediator = provider.GetRequiredService<IMediator>();

        // act
        var result = await mediator.ExecuteAsync(new MediatorTestRequest(), CancellationToken.None);

        // assert
        Assert.Equal("ok", result);
    }

    [Fact]
    public async Task ExecuteAsync_CalledTwice_ReturnsSameResult() {
        // arrange
        var provider = BuildProvider(s =>
            s.AddTransient<IRequestHandler<MediatorTestRequest, string>, MediatorTestRequestHandler>()
        );
        var mediator = provider.GetRequiredService<IMediator>();

        // act
        var first = await mediator.ExecuteAsync(new MediatorTestRequest(), CancellationToken.None);
        var second = await mediator.ExecuteAsync(new MediatorTestRequest(), CancellationToken.None);

        // assert
        Assert.Multiple(() => {
            Assert.Equal("ok", first);
            Assert.Equal("ok", second);
        });
    }

    [Fact]
    public async Task PublishAsync_WithRegisteredHandler_InvokesHandler() {
        // arrange
        MediatorTestEventHandler.Reset();
        var provider = BuildProvider(s =>
            s.AddTransient<IEventHandler<MediatorTestEvent>, MediatorTestEventHandler>()
        );
        var mediator = provider.GetRequiredService<IMediator>();

        // act
        await mediator.PublishAsync(new MediatorTestEvent(), CancellationToken.None);

        // assert
        Assert.Equal(1, MediatorTestEventHandler.InvocationCount);
    }

    [Fact]
    public async Task PublishAsync_WithMultipleHandlers_InvokesAll() {
        // arrange
        MediatorTestEventHandler.Reset();
        MediatorSecondTestEventHandler.Reset();
        var provider = BuildProvider(s => {
            s.AddTransient<IEventHandler<MediatorTestEvent>, MediatorTestEventHandler>();
            s.AddTransient<IEventHandler<MediatorTestEvent>, MediatorSecondTestEventHandler>();
        });
        var mediator = provider.GetRequiredService<IMediator>();

        // act
        await mediator.PublishAsync(new MediatorTestEvent(), CancellationToken.None);

        // assert
        Assert.Multiple(() => {
            Assert.Equal(1, MediatorTestEventHandler.InvocationCount);
            Assert.Equal(1, MediatorSecondTestEventHandler.InvocationCount);
        });
    }

    [Fact]
    public async Task CreateAsync_WithRegisteredHandler_YieldsAllItems() {
        // arrange
        var provider = BuildProvider(s =>
            s.AddTransient<IStreamHandler<MediatorTestStream, int>, MediatorTestStreamHandler>()
        );
        var mediator = provider.GetRequiredService<IMediator>();
        var collected = new List<int>();

        // act
        await foreach (var item in mediator.CreateAsync<int>(new MediatorTestStream(), CancellationToken.None)) {
            collected.Add(item);
        }

        // assert
        Assert.Equal([1, 2, 3], collected);
    }
}

// ---------------------------------------------------------------------------
// Test types — declared at namespace level so Moq/Castle.DynamicProxy can
// proxy ILogger<T> where T references these types, and so that DI can
// resolve closed generic service descriptors without FullName being null.
// ---------------------------------------------------------------------------

public record MediatorTestRequest : IRequest<string>;

public class MediatorTestRequestHandler : IRequestHandler<MediatorTestRequest, string> {
    public Task<string> HandleAsync(MediatorTestRequest request, CancellationToken cancellationToken) {
        return Task.FromResult("ok");
    }
}

public record MediatorTestEvent : IEvent;

public class MediatorTestEventHandler : IEventHandler<MediatorTestEvent> {
    public static int InvocationCount;

    public static void Reset() {
        InvocationCount = 0;
    }

    public Task HandleAsync(MediatorTestEvent evt, CancellationToken cancellationToken) {
        Interlocked.Increment(ref InvocationCount);
        return Task.CompletedTask;
    }
}

public class MediatorSecondTestEventHandler : IEventHandler<MediatorTestEvent> {
    public static int InvocationCount;

    public static void Reset() {
        InvocationCount = 0;
    }

    public Task HandleAsync(MediatorTestEvent evt, CancellationToken cancellationToken) {
        Interlocked.Increment(ref InvocationCount);
        return Task.CompletedTask;
    }
}

public record MediatorTestStream : IStream<int>;

public class MediatorTestStreamHandler : IStreamHandler<MediatorTestStream, int> {
    public async IAsyncEnumerable<int> HandleAsync(MediatorTestStream request,
        [EnumeratorCancellation] CancellationToken cancellationToken) {
        yield return 1;
        yield return 2;
        yield return 3;
        await Task.CompletedTask;
    }
}

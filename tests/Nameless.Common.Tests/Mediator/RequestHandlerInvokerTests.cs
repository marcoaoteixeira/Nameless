using Microsoft.Extensions.DependencyInjection;
using Nameless.Mediator.Requests;

namespace Nameless.Mediator;

public sealed class RequestLog {
    private readonly List<string> _entries = [];

    public IReadOnlyList<string> Entries {
        get { lock (_entries) { return [.. _entries]; } }
    }

    public CancellationToken LastToken { get; set; }

    public CancellationToken BehaviorToken { get; set; }

    public void Add(string entry) {
        lock (_entries) { _entries.Add(entry); }
    }
}

public record VoidRequest(string Name) : IRequest;

public sealed class VoidRequestHandler(RequestLog log) : IRequestHandler<VoidRequest> {
    public Task HandleAsync(VoidRequest request, CancellationToken cancellationToken) {
        log.LastToken = cancellationToken;
        log.Add($"handler:{request.Name}");

        return Task.CompletedTask;
    }
}

public record ThrowingVoidRequest : IRequest;

public sealed class ThrowingVoidRequestHandler : IRequestHandler<ThrowingVoidRequest> {
    public Task HandleAsync(ThrowingVoidRequest request, CancellationToken cancellationToken)
        => throw new InvalidOperationException("boom");
}

public record UnhandledVoidRequest : IRequest;

public sealed class VoidBehaviorA<TRequest>(RequestLog log) : IRequestPipelineBehavior<TRequest>
    where TRequest : IRequest {
    public async Task HandleAsync(TRequest request, RequestHandlerDelegate next, CancellationToken cancellationToken) {
        log.Add("A:before");
        await next(cancellationToken);
        log.Add("A:after");
    }
}

public sealed class VoidBehaviorB<TRequest>(RequestLog log) : IRequestPipelineBehavior<TRequest>
    where TRequest : IRequest {
    public async Task HandleAsync(TRequest request, RequestHandlerDelegate next, CancellationToken cancellationToken) {
        log.Add("B:before");
        await next(cancellationToken);
        log.Add("B:after");
    }
}

public sealed class ShortCircuitVoidBehavior<TRequest>(RequestLog log) : IRequestPipelineBehavior<TRequest>
    where TRequest : IRequest {
    public Task HandleAsync(TRequest request, RequestHandlerDelegate next, CancellationToken cancellationToken) {
        log.Add("short-circuit");

        return Task.CompletedTask;
    }
}

public sealed class TokenSwappingVoidBehavior<TRequest>(RequestLog log) : IRequestPipelineBehavior<TRequest>
    where TRequest : IRequest {
    public Task HandleAsync(TRequest request, RequestHandlerDelegate next, CancellationToken cancellationToken) {
        log.Add("swap");

        return next(CancellationToken.None);
    }
}

public sealed class TokenRecordingVoidBehavior<TRequest>(RequestLog log) : IRequestPipelineBehavior<TRequest>
    where TRequest : IRequest {
    public Task HandleAsync(TRequest request, RequestHandlerDelegate next, CancellationToken cancellationToken) {
        log.BehaviorToken = cancellationToken;

        return next(cancellationToken);
    }
}

public record AmbiguousRequest : IRequest<string>, IRequest<int>;

public sealed class TypedTracker {
    public int Invocations;
}

public record TrackedTypedRequest : IRequest<string>;

public sealed class TrackedTypedRequestHandler(TypedTracker tracker) : IRequestHandler<TrackedTypedRequest, string> {
    public Task<string> HandleAsync(TrackedTypedRequest request, CancellationToken cancellationToken) {
        Interlocked.Increment(ref tracker.Invocations);

        return Task.FromResult("typed");
    }
}

[UnitTest]
public class RequestHandlerInvokerTests {
    private static ServiceProvider BuildProvider(Action<IServiceCollection>? configure = null) {
        var services = new ServiceCollection();
        services.AddSingleton<RequestLog>();
        services.AddSingleton<TypedTracker>();
        services.AddTransient<IRequestHandler<VoidRequest>, VoidRequestHandler>();
        services.AddTransient<IRequestHandler<ThrowingVoidRequest>, ThrowingVoidRequestHandler>();
        services.AddTransient<IRequestHandler<TrackedTypedRequest, string>, TrackedTypedRequestHandler>();
        configure?.Invoke(services);

        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task ExecuteAsync_VoidRequest_InvokesHandler() {
        // arrange
        using var provider = BuildProvider();
        var sut = new RequestHandlerInvoker(provider);

        // act
        await sut.ExecuteAsync(new VoidRequest("one"), TestContext.Current.CancellationToken);

        // assert
        Assert.Equal(["handler:one"], provider.GetRequiredService<RequestLog>().Entries);
    }

    [Fact]
    public async Task ExecuteAsync_VoidRequest_PassesCancellationTokenToHandler() {
        // arrange
        using var provider = BuildProvider();
        var sut = new RequestHandlerInvoker(provider);
        using var cts = new CancellationTokenSource();

        // act
        await sut.ExecuteAsync(new VoidRequest("one"), cts.Token);

        // assert
        Assert.Equal(cts.Token, provider.GetRequiredService<RequestLog>().LastToken);
    }

    [Fact]
    public async Task ExecuteAsync_VoidRequest_RunsBehaviorsOutermostFirstInRegistrationOrder() {
        // arrange
        using var provider = BuildProvider(services => {
            services.AddTransient(typeof(IRequestPipelineBehavior<>), typeof(VoidBehaviorA<>));
            services.AddTransient(typeof(IRequestPipelineBehavior<>), typeof(VoidBehaviorB<>));
        });
        var sut = new RequestHandlerInvoker(provider);

        // act
        await sut.ExecuteAsync(new VoidRequest("one"), TestContext.Current.CancellationToken);

        // assert
        Assert.Equal(
            ["A:before", "B:before", "handler:one", "B:after", "A:after"],
            provider.GetRequiredService<RequestLog>().Entries
        );
    }

    [Fact]
    public async Task ExecuteAsync_VoidRequest_WhenBehaviorShortCircuits_DoesNotInvokeHandler() {
        // arrange
        using var provider = BuildProvider(services =>
            services.AddTransient(typeof(IRequestPipelineBehavior<>), typeof(ShortCircuitVoidBehavior<>)));
        var sut = new RequestHandlerInvoker(provider);

        // act
        await sut.ExecuteAsync(new VoidRequest("one"), TestContext.Current.CancellationToken);

        // assert
        Assert.Equal(["short-circuit"], provider.GetRequiredService<RequestLog>().Entries);
    }

    [Fact]
    public async Task ExecuteAsync_VoidRequest_WhenOuterBehaviorPassesNoToken_InnerBehaviorReceivesOriginalToken() {
        // arrange
        using var provider = BuildProvider(services => {
            services.AddTransient(typeof(IRequestPipelineBehavior<>), typeof(TokenSwappingVoidBehavior<>));
            services.AddTransient(typeof(IRequestPipelineBehavior<>), typeof(TokenRecordingVoidBehavior<>));
        });
        var sut = new RequestHandlerInvoker(provider);
        using var cts = new CancellationTokenSource();

        // act
        await sut.ExecuteAsync(new VoidRequest("one"), cts.Token);

        // assert
        Assert.Equal(cts.Token, provider.GetRequiredService<RequestLog>().BehaviorToken);
    }

    [Fact]
    public async Task ExecuteAsync_VoidRequest_WhenHandlerThrows_PropagatesException() {
        // arrange
        using var provider = BuildProvider();
        var sut = new RequestHandlerInvoker(provider);

        // act & assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.ExecuteAsync(new ThrowingVoidRequest(), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ExecuteAsync_VoidRequest_WithoutRegisteredHandler_Throws() {
        // arrange
        using var provider = BuildProvider();
        var sut = new RequestHandlerInvoker(provider);

        // act & assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.ExecuteAsync(new UnhandledVoidRequest(), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ExecuteAsync_VoidRequest_WithNullRequest_Throws() {
        // arrange
        using var provider = BuildProvider();
        var sut = new RequestHandlerInvoker(provider);

        // act & assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => sut.ExecuteAsync((IRequest)null!, TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ExecuteAsync_VoidRequest_CalledRepeatedly_ReusesCachedWrapper() {
        // arrange
        using var provider = BuildProvider();
        var sut = new RequestHandlerInvoker(provider);

        // act
        await sut.ExecuteAsync(new VoidRequest("1"), TestContext.Current.CancellationToken);
        await sut.ExecuteAsync(new VoidRequest("2"), TestContext.Current.CancellationToken);

        // assert
        Assert.Equal(["handler:1", "handler:2"], provider.GetRequiredService<RequestLog>().Entries);
    }

    // ── typed requests through the void overload (D1) ─────────────────────────

    [Fact]
    public async Task ExecuteAsync_TypedRequestThroughVoidOverload_RunsTypedHandlerAndDiscardsResult() {
        // arrange
        using var provider = BuildProvider();
        var sut = new RequestHandlerInvoker(provider);
        IRequest request = new TrackedTypedRequest();

        // act
        await sut.ExecuteAsync(request, TestContext.Current.CancellationToken);

        // assert
        Assert.Equal(1, provider.GetRequiredService<TypedTracker>().Invocations);
    }

    [Fact]
    public async Task ExecuteAsync_VoidOverloadThenTypedOverload_ForSameRequestType_ShareOneWrapper() {
        // arrange
        using var provider = BuildProvider();
        var sut = new RequestHandlerInvoker(provider);

        // act
        await sut.ExecuteAsync((IRequest)new TrackedTypedRequest(), TestContext.Current.CancellationToken);
        var response = await sut.ExecuteAsync(new TrackedTypedRequest(), TestContext.Current.CancellationToken);

        // assert
        Assert.Multiple(
            () => Assert.Equal("typed", response),
            () => Assert.Equal(2, provider.GetRequiredService<TypedTracker>().Invocations)
        );
    }

    [Fact]
    public async Task ExecuteAsync_TypedOverloadThenVoidOverload_ForSameRequestType_ShareOneWrapper() {
        // arrange
        using var provider = BuildProvider();
        var sut = new RequestHandlerInvoker(provider);

        // act
        var response = await sut.ExecuteAsync(new TrackedTypedRequest(), TestContext.Current.CancellationToken);
        await sut.ExecuteAsync((IRequest)new TrackedTypedRequest(), TestContext.Current.CancellationToken);

        // assert
        Assert.Multiple(
            () => Assert.Equal("typed", response),
            () => Assert.Equal(2, provider.GetRequiredService<TypedTracker>().Invocations)
        );
    }

    [Fact]
    public async Task ExecuteAsync_RequestImplementingSeveralResponseTypes_ThrowsInvalidOperationException() {
        // arrange
        using var provider = BuildProvider();
        var sut = new RequestHandlerInvoker(provider);

        // act & assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.ExecuteAsync((IRequest)new AmbiguousRequest(), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ExecuteAsync_TypedRequestViewedAsCovariantResponse_ThrowsInvalidCastException() {
        // arrange
        // IRequest<out TResponse> is covariant, but the cached wrapper is closed over the
        // request's declared response type (string), so asking for object is not supported.
        using var provider = BuildProvider();
        var sut = new RequestHandlerInvoker(provider);
        IRequest<object> covariant = new TrackedTypedRequest();

        // act & assert
        await Assert.ThrowsAsync<InvalidCastException>(
            () => sut.ExecuteAsync(covariant, TestContext.Current.CancellationToken));
    }

    // ── through IMediator ─────────────────────────────────────────────────────

    [Fact]
    public async Task IMediator_ExecuteAsync_VoidRequest_DispatchesToHandler() {
        // arrange
        var services = new ServiceCollection();
        services.AddSingleton<RequestLog>();
        services.RegisterMediator(r => r
            .WithUseAssemblyScan(false)
            .WithRequestHandler<VoidRequestHandler, VoidRequest>());
        using var provider = services.BuildServiceProvider();

        // act
        await provider.GetRequiredService<IMediator>().ExecuteAsync(new VoidRequest("mediated"), TestContext.Current.CancellationToken);

        // assert
        Assert.Equal(["handler:mediated"], provider.GetRequiredService<RequestLog>().Entries);
    }
}

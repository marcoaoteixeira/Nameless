using Microsoft.Extensions.DependencyInjection;
using Nameless.Mediator.Requests;

namespace Nameless.Mediator;

public record OrderedRequest : IRequest<string>;

public record OtherRequest : IRequest<string>;

public record OrderedVoidRequest : IRequest;

public record OtherVoidRequest : IRequest;

public sealed class OrderedRequestHandler(RequestLog log) : IRequestHandler<OrderedRequest, string> {
    public Task<string> HandleAsync(OrderedRequest request, CancellationToken cancellationToken) {
        log.Add("handler");

        return Task.FromResult("ok");
    }
}

public sealed class OtherRequestHandler(RequestLog log) : IRequestHandler<OtherRequest, string> {
    public Task<string> HandleAsync(OtherRequest request, CancellationToken cancellationToken) {
        log.Add("handler");

        return Task.FromResult("ok");
    }
}

public sealed class OrderedVoidRequestHandler(RequestLog log) : IRequestHandler<OrderedVoidRequest> {
    public Task HandleAsync(OrderedVoidRequest request, CancellationToken cancellationToken) {
        log.Add("handler");

        return Task.CompletedTask;
    }
}

public sealed class OtherVoidRequestHandler(RequestLog log) : IRequestHandler<OtherVoidRequest> {
    public Task HandleAsync(OtherVoidRequest request, CancellationToken cancellationToken) {
        log.Add("handler");

        return Task.CompletedTask;
    }
}

public abstract class TracingTypedBehavior<TRequest, TResponse>(RequestLog log, string name)
    : IRequestPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse> {
    public Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken) {
        log.Add(name);

        return next(cancellationToken);
    }
}

public abstract class TracingVoidBehavior<TRequest>(RequestLog log, string name)
    : IRequestPipelineBehavior<TRequest>
    where TRequest : IRequest {
    public Task HandleAsync(TRequest request, RequestHandlerDelegate next, CancellationToken cancellationToken) {
        log.Add(name);

        return next(cancellationToken);
    }
}

public sealed class OpenTypedFirst<TRequest, TResponse>(RequestLog log) : TracingTypedBehavior<TRequest, TResponse>(log, "open-1")
    where TRequest : IRequest<TResponse>;

public sealed class OpenTypedSecond<TRequest, TResponse>(RequestLog log) : TracingTypedBehavior<TRequest, TResponse>(log, "open-2")
    where TRequest : IRequest<TResponse>;

public sealed class ClosedTypedFirst(RequestLog log) : TracingTypedBehavior<OrderedRequest, string>(log, "closed-1");

public sealed class ClosedTypedSecond(RequestLog log) : TracingTypedBehavior<OrderedRequest, string>(log, "closed-2");

public sealed class ClosedTypedOther(RequestLog log) : TracingTypedBehavior<OtherRequest, string>(log, "closed-other");

public sealed class OpenVoidFirst<TRequest>(RequestLog log) : TracingVoidBehavior<TRequest>(log, "open-1")
    where TRequest : IRequest;

public sealed class OpenVoidSecond<TRequest>(RequestLog log) : TracingVoidBehavior<TRequest>(log, "open-2")
    where TRequest : IRequest;

public sealed class ClosedVoidFirst(RequestLog log) : TracingVoidBehavior<OrderedVoidRequest>(log, "closed-1");

public sealed class ClosedVoidSecond(RequestLog log) : TracingVoidBehavior<OrderedVoidRequest>(log, "closed-2");

public sealed class ClosedVoidOther(RequestLog log) : TracingVoidBehavior<OtherVoidRequest>(log, "closed-other");

/// <summary>
///     Verifies, through the real <c>RegisterMediator</c> registration, that
///     pipeline behaviors run in registration order and that open generic
///     behaviors apply to every request while closed ones apply only to
///     their own request type.
/// </summary>
[IntegrationTest]
public class PipelineBehaviorOrderingTests {
    private static ServiceProvider BuildProvider(Action<MediatorRegistration> configure) {
        var services = new ServiceCollection();
        services.AddSingleton<RequestLog>();
        services.RegisterMediator(registration => {
            registration.WithUseAssemblyScan(false);
            configure(registration);
        });

        return services.BuildServiceProvider();
    }

    private static void RegisterTyped(MediatorRegistration registration) {
        registration.WithRequestHandler(typeof(OrderedRequestHandler))
                    .WithRequestHandler(typeof(OtherRequestHandler))
                    .WithRequestPipelineBehavior(typeof(OpenTypedFirst<,>))
                    .WithRequestPipelineBehavior(typeof(ClosedTypedFirst))
                    .WithRequestPipelineBehavior(typeof(OpenTypedSecond<,>))
                    .WithRequestPipelineBehavior(typeof(ClosedTypedSecond))
                    .WithRequestPipelineBehavior(typeof(ClosedTypedOther));
    }

    private static void RegisterVoid(MediatorRegistration registration) {
        registration.WithRequestHandler(typeof(OrderedVoidRequestHandler))
                    .WithRequestHandler(typeof(OtherVoidRequestHandler))
                    .WithRequestPipelineBehavior(typeof(OpenVoidFirst<>))
                    .WithRequestPipelineBehavior(typeof(ClosedVoidFirst))
                    .WithRequestPipelineBehavior(typeof(OpenVoidSecond<>))
                    .WithRequestPipelineBehavior(typeof(ClosedVoidSecond))
                    .WithRequestPipelineBehavior(typeof(ClosedVoidOther));
    }

    [Fact]
    public async Task ExecuteAsync_TypedRequest_RunsOpenAndClosedBehaviorsInRegistrationOrder() {
        // arrange
        using var provider = BuildProvider(RegisterTyped);
        var sut = provider.GetRequiredService<IMediator>();

        // act
        await sut.ExecuteAsync(new OrderedRequest(), TestContext.Current.CancellationToken);

        // assert
        Assert.Equal(
            ["open-1", "closed-1", "open-2", "closed-2", "handler"],
            provider.GetRequiredService<RequestLog>().Entries
        );
    }

    [Fact]
    public async Task ExecuteAsync_TypedRequest_SkipsClosedBehaviorsOfOtherRequests() {
        // arrange
        using var provider = BuildProvider(RegisterTyped);
        var sut = provider.GetRequiredService<IMediator>();

        // act
        await sut.ExecuteAsync(new OtherRequest(), TestContext.Current.CancellationToken);

        // assert
        Assert.Equal(
            ["open-1", "open-2", "closed-other", "handler"],
            provider.GetRequiredService<RequestLog>().Entries
        );
    }

    [Fact]
    public async Task ExecuteAsync_VoidRequest_RunsOpenAndClosedBehaviorsInRegistrationOrder() {
        // arrange
        using var provider = BuildProvider(RegisterVoid);
        var sut = provider.GetRequiredService<IMediator>();

        // act
        await sut.ExecuteAsync(new OrderedVoidRequest(), TestContext.Current.CancellationToken);

        // assert
        Assert.Equal(
            ["open-1", "closed-1", "open-2", "closed-2", "handler"],
            provider.GetRequiredService<RequestLog>().Entries
        );
    }

    [Fact]
    public async Task ExecuteAsync_VoidRequest_SkipsClosedBehaviorsOfOtherRequests() {
        // arrange
        using var provider = BuildProvider(RegisterVoid);
        var sut = provider.GetRequiredService<IMediator>();

        // act
        await sut.ExecuteAsync(new OtherVoidRequest(), TestContext.Current.CancellationToken);

        // assert
        Assert.Equal(
            ["open-1", "open-2", "closed-other", "handler"],
            provider.GetRequiredService<RequestLog>().Entries
        );
    }

    [Fact]
    public async Task ExecuteAsync_ClosedBehaviorRegisteredBeforeOpen_RunsClosedFirst() {
        // arrange
        using var provider = BuildProvider(registration => registration
            .WithRequestHandler(typeof(OrderedRequestHandler))
            .WithRequestPipelineBehavior(typeof(ClosedTypedFirst))
            .WithRequestPipelineBehavior(typeof(OpenTypedFirst<,>)));
        var sut = provider.GetRequiredService<IMediator>();

        // act
        await sut.ExecuteAsync(new OrderedRequest(), TestContext.Current.CancellationToken);

        // assert
        Assert.Equal(
            ["closed-1", "open-1", "handler"],
            provider.GetRequiredService<RequestLog>().Entries
        );
    }
}

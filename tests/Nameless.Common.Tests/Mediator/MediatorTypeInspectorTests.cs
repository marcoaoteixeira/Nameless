using Nameless.Mediator.Events;
using Nameless.Mediator.Requests;
using Nameless.Mediator.Streams;

namespace Nameless.Mediator;

// Fixtures are internal on purpose: several are invalid and must never be
// picked up by the assembly scan tests that scan this test assembly.

internal record InspA : IRequest<string>;
internal record InspB : IRequest;
internal record InspC : IRequest<int>;
internal record InspCov : IRequest<string>;
internal record InspAmbiguous : IRequest<string>, IRequest<int>;
internal record InspGenericRequest<T> : IRequest<T>;
internal record InspEvent : IEvent;
internal record InspWrappedEvent<T> : IEvent;
internal record InspStream : IStream<string>;
internal record InspMultiStream : IStream<string>, IStream<int>;

internal class InspMultiHandler : IRequestHandler<InspA, string>, IRequestHandler<InspB>, IRequestHandler<InspC, int> {
    public Task<string> HandleAsync(InspA request, CancellationToken cancellationToken) => Task.FromResult("a");
    public Task HandleAsync(InspB request, CancellationToken cancellationToken) => Task.CompletedTask;
    public Task<int> HandleAsync(InspC request, CancellationToken cancellationToken) => Task.FromResult(1);
}

internal class InspMultiWithVoidForResponseHandler : IRequestHandler<InspA, string>, IRequestHandler<InspA> {
    public Task<string> HandleAsync(InspA request, CancellationToken cancellationToken) => Task.FromResult("a");
    Task IRequestHandler<InspA>.HandleAsync(InspA request, CancellationToken cancellationToken) => Task.CompletedTask;
}

internal class InspVoidForResponseHandler : IRequestHandler<InspA> {
    public Task HandleAsync(InspA request, CancellationToken cancellationToken) => Task.CompletedTask;
}

internal class InspCovariantHandler : IRequestHandler<InspCov, object> {
    public Task<object> HandleAsync(InspCov request, CancellationToken cancellationToken) => Task.FromResult<object>("x");
}

internal class InspAmbiguousHandler : IRequestHandler<InspAmbiguous, string> {
    public Task<string> HandleAsync(InspAmbiguous request, CancellationToken cancellationToken) => Task.FromResult("x");
}

internal class InspOpenHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse> {
    public Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken) => Task.FromResult(default(TResponse)!);
}

internal class InspOpenVoidHandler<TRequest> : IRequestHandler<TRequest>
    where TRequest : IRequest {
    public Task HandleAsync(TRequest request, CancellationToken cancellationToken) => Task.CompletedTask;
}

internal class InspWrappedArityHandler<T> : IRequestHandler<InspGenericRequest<T>, T> {
    public Task<T> HandleAsync(InspGenericRequest<T> request, CancellationToken cancellationToken) => Task.FromResult(default(T)!);
}

internal class InspSwappedHandler<TResponse, TRequest> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse> {
    public Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken) => Task.FromResult(default(TResponse)!);
}

internal class InspOuter<T> {
    internal class Inner : IRequestHandler<InspB> {
        public Task HandleAsync(InspB request, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}

internal class InspEventHandler : IEventHandler<InspEvent> {
    public Task HandleAsync(InspEvent evt, CancellationToken cancellationToken) => Task.CompletedTask;
}

internal class InspOpenEventHandler<TEvent> : IEventHandler<TEvent>
    where TEvent : IEvent {
    public Task HandleAsync(TEvent evt, CancellationToken cancellationToken) => Task.CompletedTask;
}

internal class InspWrappedEventHandler<T> : IEventHandler<InspWrappedEvent<T>> {
    public Task HandleAsync(InspWrappedEvent<T> evt, CancellationToken cancellationToken) => Task.CompletedTask;
}

internal class InspCovariantStreamHandler : IStreamHandler<InspStream, object> {
    public IAsyncEnumerable<object> HandleAsync(InspStream request, CancellationToken cancellationToken) => AsyncEnumerable.Empty<object>();
}

internal class InspMultiStreamHandler : IStreamHandler<InspMultiStream, string>, IStreamHandler<InspMultiStream, int> {
    public IAsyncEnumerable<string> HandleAsync(InspMultiStream request, CancellationToken cancellationToken) => AsyncEnumerable.Empty<string>();
    IAsyncEnumerable<int> IStreamHandler<InspMultiStream, int>.HandleAsync(InspMultiStream request, CancellationToken cancellationToken) => AsyncEnumerable.Empty<int>();
}

internal class InspVoidBehaviorForResponse : IRequestPipelineBehavior<InspA> {
    public Task HandleAsync(InspA request, RequestHandlerDelegate next, CancellationToken cancellationToken) => next(cancellationToken);
}

internal class InspCovariantBehavior : IRequestPipelineBehavior<InspCov, object> {
    public Task<object> HandleAsync(InspCov request, RequestHandlerDelegate<object> next, CancellationToken cancellationToken) => next(cancellationToken);
}

internal class InspWrongResponseStreamBehavior : IStreamPipelineBehavior<InspStream, int> {
    public IAsyncEnumerable<int> HandleAsync(InspStream request, StreamHandlerDelegate<int> next, CancellationToken cancellationToken) => next();
}

internal class InspWrappedArityBehavior<T> : IRequestPipelineBehavior<InspGenericRequest<T>, T> {
    public Task<T> HandleAsync(InspGenericRequest<T> request, RequestHandlerDelegate<T> next, CancellationToken cancellationToken) => next(cancellationToken);
}

[UnitTest]
public class MediatorTypeInspectorTests {
    [Fact]
    public void GetServiceTypes_ClosedHandlerForSeveralRequests_ReturnsEveryClosedInterfaceOfThatService() {
        // act
        var typed = MediatorTypeInspector.GetServiceTypes(typeof(InspMultiHandler), typeof(IRequestHandler<,>));
        var @void = MediatorTypeInspector.GetServiceTypes(typeof(InspMultiHandler), typeof(IRequestHandler<>));

        // assert
        Assert.Multiple(
            () => Assert.Equivalent(new[] { typeof(IRequestHandler<InspA, string>), typeof(IRequestHandler<InspC, int>) }, typed, strict: true),
            () => Assert.Equal([typeof(IRequestHandler<InspB>)], @void)
        );
    }

    [Fact]
    public void GetServiceTypes_TypeNotImplementingService_ReturnsEmpty() {
        // act
        var result = MediatorTypeInspector.GetServiceTypes(typeof(InspOpenVoidHandler<>), typeof(IRequestHandler<,>));

        // assert
        Assert.Empty(result);
    }

    [Theory]
    [InlineData(typeof(InspOpenHandler<,>), typeof(IRequestHandler<,>))]
    [InlineData(typeof(InspOpenVoidHandler<>), typeof(IRequestHandler<>))]
    [InlineData(typeof(InspOpenEventHandler<>), typeof(IEventHandler<>))]
    public void GetServiceTypes_OpenGenericWithMatchingShape_ReturnsOpenService(Type implementation, Type service) {
        // act
        var result = MediatorTypeInspector.GetServiceTypes(implementation, service);

        // assert
        Assert.Equal([service], result);
    }

    [Fact]
    public void GetServiceTypes_ClosedEventHandler_ReturnsClosedInterface() {
        // act
        var result = MediatorTypeInspector.GetServiceTypes(typeof(InspEventHandler), typeof(IEventHandler<>));

        // assert
        Assert.Equal([typeof(IEventHandler<InspEvent>)], result);
    }

    [Fact]
    public void GetServiceTypes_StreamHandlerForEveryDeclaredResponse_ReturnsBothInterfaces() {
        // act
        var result = MediatorTypeInspector.GetServiceTypes(typeof(InspMultiStreamHandler), typeof(IStreamHandler<,>));

        // assert
        Assert.Equivalent(new[] { typeof(IStreamHandler<InspMultiStream, string>), typeof(IStreamHandler<InspMultiStream, int>) }, result, strict: true);
    }

    [Theory]
    // open generics whose type parameters don't map one-to-one onto the service
    [InlineData(typeof(InspWrappedArityHandler<>), typeof(IRequestHandler<,>))]
    [InlineData(typeof(InspSwappedHandler<,>), typeof(IRequestHandler<,>))]
    [InlineData(typeof(InspOuter<>.Inner), typeof(IRequestHandler<>))]
    [InlineData(typeof(InspWrappedEventHandler<>), typeof(IEventHandler<>))]
    [InlineData(typeof(InspWrappedArityBehavior<>), typeof(IRequestPipelineBehavior<,>))]
    // void handler/behavior for a request that returns a response
    [InlineData(typeof(InspVoidForResponseHandler), typeof(IRequestHandler<>))]
    [InlineData(typeof(InspMultiWithVoidForResponseHandler), typeof(IRequestHandler<>))]
    [InlineData(typeof(InspVoidBehaviorForResponse), typeof(IRequestPipelineBehavior<>))]
    // response type differs from the one the request declares
    [InlineData(typeof(InspCovariantHandler), typeof(IRequestHandler<,>))]
    [InlineData(typeof(InspAmbiguousHandler), typeof(IRequestHandler<,>))]
    [InlineData(typeof(InspCovariantBehavior), typeof(IRequestPipelineBehavior<,>))]
    [InlineData(typeof(InspCovariantStreamHandler), typeof(IStreamHandler<,>))]
    [InlineData(typeof(InspWrongResponseStreamBehavior), typeof(IStreamPipelineBehavior<,>))]
    public void GetServiceTypes_InvalidType_ThrowsInvalidOperationExceptionNamingTheType(Type implementation, Type service) {
        // act
        var exception = Assert.Throws<InvalidOperationException>(() => MediatorTypeInspector.GetServiceTypes(implementation, service));

        // assert
        Assert.Contains(implementation.GetPrettyName(), exception.Message);
    }

    [Theory]
    [InlineData(typeof(InspWrappedArityHandler<>), typeof(IRequestHandler<,>), "InspWrappedArityHandler<TRequest,TResponse> : IRequestHandler<TRequest,TResponse>")]
    [InlineData(typeof(InspWrappedEventHandler<>), typeof(IEventHandler<>), "InspWrappedEventHandler<TEvent> : IEventHandler<TEvent>")]
    [InlineData(typeof(InspOuter<>.Inner), typeof(IRequestHandler<>), "Inner<TRequest> : IRequestHandler<TRequest>")]
    public void GetServiceTypes_OpenGenericWithMismatchedTypeParameters_SuggestsTheExpectedDeclaration(Type implementation, Type service, string expected) {
        // act
        var exception = Assert.Throws<InvalidOperationException>(() => MediatorTypeInspector.GetServiceTypes(implementation, service));

        // assert
        Assert.Contains(expected, exception.Message);
    }
}

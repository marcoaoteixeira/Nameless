using Nameless.Mediator.Requests;
using Nameless.Mediator.Streams;

namespace Nameless.Mediator;

public class PassThroughRequestBehavior<TRequest, TResponse> : IRequestPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull {
    public Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        => next(cancellationToken);
}

public class PassThroughStreamBehavior<TRequest, TResponse> : IStreamPipelineBehavior<TRequest, TResponse>
    where TRequest : IStream<TResponse> {
    public IAsyncEnumerable<TResponse> HandleAsync(TRequest request, StreamHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        => next();
}

public class GenericEventHandler<TEvent> : Events.IEventHandler<TEvent> where TEvent : Events.IEvent {
    public Task HandleAsync(TEvent evt, CancellationToken cancellationToken) => Task.CompletedTask;
}

[UnitTest]
public class MediatorRegistrationTests {
    private static MediatorRegistration CreateSut() => new MediatorRegistration().WithUseAssemblyScan(false);

    [Fact]
    public void WithEventHandler_Generic_AddsType() {
        // arrange
        var sut = CreateSut();

        // act
        var returned = sut.WithEventHandler<MediatorTestEventHandler, MediatorTestEvent>();

        // assert
        Assert.Multiple(
            () => Assert.Same(sut, returned),
            () => Assert.Contains(typeof(MediatorTestEventHandler), sut.EventHandlers)
        );
    }

    [Fact]
    public void WithEventHandler_WithUnrelatedType_Throws() {
        // act & assert
        Assert.Throws<ArgumentException>(() => CreateSut().WithEventHandler(typeof(string)));
    }

    [Fact]
    public void WithRequestHandler_Generic_AddsType() {
        // arrange
        var sut = CreateSut();

        // act
        sut.WithRequestHandler<MediatorTestRequestHandler, MediatorTestRequest, string>();

        // assert
        Assert.Contains(typeof(MediatorTestRequestHandler), sut.RequestHandlers);
    }

    [Fact]
    public void WithRequestHandler_WithUnrelatedType_Throws() {
        // act & assert
        Assert.Throws<ArgumentException>(() => CreateSut().WithRequestHandler(typeof(string)));
    }

    [Fact]
    public void WithStreamHandler_Generic_AddsType() {
        // arrange
        var sut = CreateSut();

        // act
        sut.WithStreamHandler<MediatorTestStreamHandler, MediatorTestStream, int>();

        // assert
        Assert.Contains(typeof(MediatorTestStreamHandler), sut.StreamHandlers);
    }

    [Fact]
    public void WithStreamHandler_WithUnrelatedType_Throws() {
        // act & assert
        Assert.Throws<ArgumentException>(() => CreateSut().WithStreamHandler(typeof(string)));
    }

    [Fact]
    public void WithRequestPipelineBehavior_AddsOnce() {
        // arrange
        var sut = CreateSut();

        // act
        sut.WithRequestPipelineBehavior(typeof(PassThroughRequestBehavior<,>));
        sut.WithRequestPipelineBehavior(typeof(PassThroughRequestBehavior<,>));

        // assert
        Assert.Single(sut.RequestPipelineBehaviors);
    }

    [Fact]
    public void WithRequestPipelineBehavior_Generic_AddsType() {
        // arrange
        var sut = CreateSut();

        // act
        sut.WithRequestPipelineBehavior<PassThroughRequestBehavior<MediatorTestRequest, string>, MediatorTestRequest, string>();

        // assert
        Assert.Contains(typeof(PassThroughRequestBehavior<MediatorTestRequest, string>), sut.RequestPipelineBehaviors);
    }

    [Fact]
    public void WithRequestPipelineBehavior_WithUnrelatedType_Throws() {
        // act & assert
        Assert.Throws<ArgumentException>(() => CreateSut().WithRequestPipelineBehavior(typeof(string)));
    }

    [Fact]
    public void WithStreamPipelineBehavior_AddsOnce() {
        // arrange
        var sut = CreateSut();

        // act
        sut.WithStreamPipelineBehavior(typeof(PassThroughStreamBehavior<,>));
        sut.WithStreamPipelineBehavior(typeof(PassThroughStreamBehavior<,>));

        // assert
        Assert.Single(sut.StreamPipelineBehaviors);
    }

    [Fact]
    public void WithStreamPipelineBehavior_Generic_AddsType() {
        // arrange
        var sut = CreateSut();

        // act
        sut.WithStreamPipelineBehavior<PassThroughStreamBehavior<MediatorTestStream, int>, MediatorTestStream, int>();

        // assert
        Assert.Contains(typeof(PassThroughStreamBehavior<MediatorTestStream, int>), sut.StreamPipelineBehaviors);
    }

    [Fact]
    public void WithStreamPipelineBehavior_WithUnrelatedType_Throws() {
        // act & assert
        Assert.Throws<ArgumentException>(() => CreateSut().WithStreamPipelineBehavior(typeof(string)));
    }

    [Fact]
    public void WithValidatePipelineBehaviors_SetFlags() {
        // arrange
        var sut = CreateSut();

        // act
        sut.WithValidateRequestPipelineBehavior(true).WithValidateStreamPipelineBehavior(true);

        // assert
        Assert.Multiple(
            () => Assert.True(sut.UseValidateRequestPipelineBehavior),
            () => Assert.True(sut.UseValidateStreamPipelineBehavior)
        );
    }
}

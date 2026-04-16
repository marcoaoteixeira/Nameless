using Nameless.Mediator.Events;
using Nameless.Mediator.Requests;
using Nameless.Mediator.Streams;
using Nameless.Registration;

namespace Nameless.Mediator;

/// <summary>
///     Mediator options for configuring the mediator services.
/// </summary>
public class MediatorRegistration : AssemblyScanAware<MediatorRegistration> {
    private readonly HashSet<Type> _eventHandlers = [];

    private readonly HashSet<Type> _requestHandlers = [];
    private readonly List<Type> _requestPipelineBehaviors = [];
    
    private readonly HashSet<Type> _streamHandlers = [];
    private readonly List<Type> _streamPipelineBehaviors = [];

    /// <summary>
    ///     Gets the registered event handlers.
    /// </summary>
    public IReadOnlyCollection<Type> EventHandlers => _eventHandlers;

    /// <summary>
    ///     Gets the registered request handlers.
    /// </summary>
    public IReadOnlyCollection<Type> RequestHandlers => _requestHandlers;

    /// <summary>
    ///     Gets the registered request pipeline behaviors.
    /// </summary>
    public IReadOnlyCollection<Type> RequestPipelineBehaviors => _requestPipelineBehaviors;

    /// <summary>
    ///     Gets the registered stream handlers.
    /// </summary>
    public IReadOnlyCollection<Type> StreamHandlers => _streamHandlers;

    /// <summary>
    ///     Gets the registered stream pipeline behaviors.
    /// </summary>
    public IReadOnlyList<Type> StreamPipelineBehaviors => _streamPipelineBehaviors;
    
    public MediatorRegistration RegisterEventHandler<TEventHandler, TEvent>()
        where TEventHandler : IEventHandler<TEvent>
        where TEvent : IEvent {
        return RegisterEventHandler(typeof(TEventHandler));
    }

    public MediatorRegistration RegisterEventHandler(Type type) {
        Throws.When.IsNonConcreteType(type);
        Throws.When.IsNotAssignableFromGeneric(type, typeof(IEventHandler<>));

        _eventHandlers.Add(type);

        return this;
    }

    public MediatorRegistration RegisterRequestHandler<TRequestHandler, TRequest, TResponse>()
        where TRequestHandler : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse> {
        return RegisterRequestHandler(typeof(TRequestHandler));
    }

    public MediatorRegistration RegisterRequestHandler(Type type) {
        Throws.When.IsNonConcreteType(type);
        Throws.When.IsNotAssignableFromGeneric(type, typeof(IRequestHandler<,>));

        _requestHandlers.Add(type);

        return this;
    }

    public MediatorRegistration RegisterRequestPipelineBehavior<TRequestPipelineBehavior, TRequest, TResponse>()
        where TRequestPipelineBehavior : IRequestPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse> {
        return RegisterRequestPipelineBehavior(typeof(TRequestPipelineBehavior));
    }

    /// <summary>
    ///     Registers a request pipeline behavior type.
    /// </summary>
    /// <param name="type">
    ///     The pipeline behavior type.
    /// </param>
    /// <returns>
    ///     The current <see cref="MediatorRegistration"/> instance so other
    ///     actions can be chained.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     if <paramref name="type"/> is not assignable from
    ///     <see cref="IRequestPipelineBehavior{TRequest,TResponse}"/>.
    /// </exception>
    /// <remarks>
    ///     We do not register request pipeline behavior automatically in the
    ///     service collection, that's because the pipeline behaviors need to
    ///     be registered in order so the execution of the pipeline is correct.
    /// </remarks>
    public MediatorRegistration RegisterRequestPipelineBehavior(Type type) {
        Throws.When.IsNonConcreteType(type);
        Throws.When.IsNotAssignableFromGeneric(type, typeof(IRequestPipelineBehavior<,>));

        if (!_requestPipelineBehaviors.Contains(type)) {
            _requestPipelineBehaviors.Add(type);
        }

        return this;
    }

    public MediatorRegistration RegisterStreamHandler<TStreamHandler, TStream, TResponse>()
        where TStreamHandler : IStreamHandler<TStream, TResponse>
        where TStream : IStream<TResponse> {
        return RegisterStreamHandler(typeof(TStreamHandler));
    }

    public MediatorRegistration RegisterStreamHandler(Type type) {
        Throws.When.IsNonConcreteType(type);
        Throws.When.IsNotAssignableFromGeneric(type, typeof(IStreamHandler<,>));

        _streamHandlers.Add(type);

        return this;
    }

    public MediatorRegistration RegisterStreamPipelineBehavior<TStreamPipelineBehavior, TStream, TResponse>()
        where TStreamPipelineBehavior : IStreamPipelineBehavior<TStream, TResponse>
        where TStream : IStream<TResponse> {
        return RegisterStreamPipelineBehavior(typeof(TStreamPipelineBehavior));
    }

    /// <summary>
    ///     Registers a stream pipeline behavior type.
    /// </summary>
    /// <param name="type">
    ///     The pipeline behavior type.
    /// </param>
    /// <returns>
    ///     The current <see cref="MediatorRegistration"/> instance so other
    ///     actions can be chained.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     if <paramref name="type"/> is not assignable from
    ///     <see cref="IStreamPipelineBehavior{TRequest,TResponse}"/>.
    /// </exception>
    /// <remarks>
    ///     We do not register request pipeline behavior automatically in
    ///     the service collection, that's because the pipeline behaviors
    ///     need to be registered in order so the execution of the pipeline
    ///     is correct.
    /// </remarks>
    public MediatorRegistration RegisterStreamPipelineBehavior(Type type) {
        Throws.When.IsNonConcreteType(type);
        Throws.When.IsNotAssignableFromGeneric(type, typeof(IStreamPipelineBehavior<,>));

        if (!_streamPipelineBehaviors.Contains(type)) {
            _streamPipelineBehaviors.Add(type);
        }

        return this;
    }
}
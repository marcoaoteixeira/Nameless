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
    public IReadOnlyCollection<Type> EventHandlers => UseAssemblyScan
        ? ExecuteAssemblyScan(typeof(IEventHandler<>), includeGenericTypeDefinition: true)
        : _eventHandlers;

    /// <summary>
    ///     Gets the registered request handlers.
    /// </summary>
    public IReadOnlyCollection<Type> RequestHandlers => UseAssemblyScan
        ? [.. ExecuteAssemblyScan(typeof(IRequestHandler<,>), includeGenericTypeDefinition: true), .. ExecuteAssemblyScan(typeof(IRequestHandler<>), includeGenericTypeDefinition: true)]
        : _requestHandlers;

    /// <summary>
    ///     Gets the registered request pipeline behaviors.
    /// </summary>
    /// <remarks>
    ///     <see cref="IRequestPipelineBehavior{TRequest,TResponse}"/> cannot
    ///     be retrieved using assembly scan since the order in which they are
    ///     registered matters.
    /// </remarks>
    public IReadOnlyCollection<Type> RequestPipelineBehaviors => _requestPipelineBehaviors;

    /// <summary>
    ///     Whether it should use the validate request pipeline behavior.
    ///     This will be the first pipeline to be executed during request.
    /// </summary>
    public bool UseValidateRequestPipelineBehavior { get; private set; }

    /// <summary>
    ///     Gets the registered stream handlers.
    /// </summary>
    public IReadOnlyCollection<Type> StreamHandlers => UseAssemblyScan
        ? ExecuteAssemblyScan(typeof(IStreamHandler<,>), includeGenericTypeDefinition: true)
        : _streamHandlers;

    /// <summary>
    ///     Gets the registered stream pipeline behaviors.
    /// </summary>
    /// <remarks>
    ///     <see cref="IStreamPipelineBehavior{TRequest,TResponse}"/> cannot
    ///     be retrieved using assembly scan since the order in which they are
    ///     registered matters.
    /// </remarks>
    public IReadOnlyList<Type> StreamPipelineBehaviors => _streamPipelineBehaviors;

    /// <summary>
    ///     Whether it should use the validate stream pipeline behavior.
    ///     This will be the first pipeline to be executed during streaming.
    /// </summary>
    public bool UseValidateStreamPipelineBehavior { get; private set; }

    /// <summary>
    ///     Registers an event handler by generic type parameters.
    /// </summary>
    /// <typeparam name="TEventHandler">The event handler type.</typeparam>
    /// <typeparam name="TEvent">The event type handled.</typeparam>
    /// <returns>
    ///     The current <see cref="MediatorRegistration"/> instance so other
    ///     actions can be chained.
    /// </returns>
    public MediatorRegistration WithEventHandler<TEventHandler, TEvent>()
        where TEventHandler : IEventHandler<TEvent>
        where TEvent : IEvent {
        return WithEventHandler(typeof(TEventHandler));
    }

    /// <summary>
    ///     Registers an event handler by type.
    /// </summary>
    /// <param name="type">The event handler type.</param>
    /// <returns>
    ///     The current <see cref="MediatorRegistration"/> instance so other
    ///     actions can be chained.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     if <paramref name="type"/> is a non-concrete type or is not assignable from
    ///     <see cref="IEventHandler{TEvent}"/>.
    /// </exception>
    public MediatorRegistration WithEventHandler(Type type) {
        Throws.When.IsNonConcreteType(type);
        Throws.When.IsNotAssignableFromGeneric(type, typeof(IEventHandler<>));

        _eventHandlers.Add(type);

        return this;
    }

    /// <summary>
    ///     Registers a request handler by generic type parameters.
    /// </summary>
    /// <typeparam name="TRequestHandler">The request handler type.</typeparam>
    /// <typeparam name="TRequest">The request type handled.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <returns>
    ///     The current <see cref="MediatorRegistration"/> instance so other
    ///     actions can be chained.
    /// </returns>
    public MediatorRegistration WithRequestHandler<TRequestHandler, TRequest, TResponse>()
        where TRequestHandler : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse> {
        return WithRequestHandler(typeof(TRequestHandler));
    }

    /// <summary>
    ///     Registers a request handler, for a request without a response,
    ///     by generic type parameters.
    /// </summary>
    /// <typeparam name="TRequestHandler">The request handler type.</typeparam>
    /// <typeparam name="TRequest">The request type handled.</typeparam>
    /// <returns>
    ///     The current <see cref="MediatorRegistration"/> instance so other
    ///     actions can be chained.
    /// </returns>
    public MediatorRegistration WithRequestHandler<TRequestHandler, TRequest>()
        where TRequestHandler : IRequestHandler<TRequest>
        where TRequest : IRequest {
        return WithRequestHandler(typeof(TRequestHandler));
    }

    /// <summary>
    ///     Registers a request handler by type.
    /// </summary>
    /// <param name="type">The request handler type.</param>
    /// <returns>
    ///     The current <see cref="MediatorRegistration"/> instance so other
    ///     actions can be chained.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     if <paramref name="type"/> is a non-concrete type or is not assignable from
    ///     <see cref="IRequestHandler{TRequest,TResponse}"/>.
    /// </exception>
    public MediatorRegistration WithRequestHandler(Type type) {
        Throws.When.IsNonConcreteType(type);
        if (!typeof(IRequestHandler<,>).IsAssignableFromGeneric(type) && !typeof(IRequestHandler<>).IsAssignableFromGeneric(type)) {
            throw new ArgumentException($"Type '{type.GetPrettyName()}' must implement '{nameof(IRequestHandler<,>)}' or '{nameof(IRequestHandler<>)}'.", nameof(type));
        }

        _requestHandlers.Add(type);

        return this;
    }

    /// <summary>
    ///     Registers a request pipeline behavior by generic type parameters.
    /// </summary>
    /// <typeparam name="TRequestPipelineBehavior">The pipeline behavior type.</typeparam>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <returns>
    ///     The current <see cref="MediatorRegistration"/> instance so other
    ///     actions can be chained.
    /// </returns>
    public MediatorRegistration WithRequestPipelineBehavior<TRequestPipelineBehavior, TRequest, TResponse>()
        where TRequestPipelineBehavior : IRequestPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse> {
        return WithRequestPipelineBehavior(typeof(TRequestPipelineBehavior));
    }

    /// <summary>
    ///     Registers a request pipeline behavior, for requests without a
    ///     response, by generic type parameters.
    /// </summary>
    /// <typeparam name="TRequestPipelineBehavior">The pipeline behavior type.</typeparam>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <returns>
    ///     The current <see cref="MediatorRegistration"/> instance so other
    ///     actions can be chained.
    /// </returns>
    public MediatorRegistration WithRequestPipelineBehavior<TRequestPipelineBehavior, TRequest>()
        where TRequestPipelineBehavior : IRequestPipelineBehavior<TRequest>
        where TRequest : IRequest {
        return WithRequestPipelineBehavior(typeof(TRequestPipelineBehavior));
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
    public MediatorRegistration WithRequestPipelineBehavior(Type type) {
        Throws.When.IsNonConcreteType(type);
        if (!typeof(IRequestPipelineBehavior<,>).IsAssignableFromGeneric(type) && !typeof(IRequestPipelineBehavior<>).IsAssignableFromGeneric(type)) {
            throw new ArgumentException($"Type '{type.GetPrettyName()}' must implement '{nameof(IRequestPipelineBehavior<,>)}' or '{nameof(IRequestPipelineBehavior<>)}'.", nameof(type));
        }

        if (!_requestPipelineBehaviors.Contains(type)) {
            _requestPipelineBehaviors.Add(type);
        }

        return this;
    }

    /// <summary>
    ///     Sets whether it should use the validate request pipeline behavior.
    /// </summary>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <returns>
    ///     The current <see cref="MediatorRegistration"/> instance so other
    ///     actions can be chained.
    /// </returns>
    public MediatorRegistration WithValidateRequestPipelineBehavior(bool value) {
        UseValidateRequestPipelineBehavior = value;

        return this;
    }

    /// <summary>
    ///     Registers a stream handler by generic type parameters.
    /// </summary>
    /// <typeparam name="TStreamHandler">The stream handler type.</typeparam>
    /// <typeparam name="TStream">The stream request type handled.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <returns>
    ///     The current <see cref="MediatorRegistration"/> instance so other
    ///     actions can be chained.
    /// </returns>
    public MediatorRegistration WithStreamHandler<TStreamHandler, TStream, TResponse>()
        where TStreamHandler : IStreamHandler<TStream, TResponse>
        where TStream : IStream<TResponse> {
        return WithStreamHandler(typeof(TStreamHandler));
    }

    /// <summary>
    ///     Registers a stream handler by type.
    /// </summary>
    /// <param name="type">The stream handler type.</param>
    /// <returns>
    ///     The current <see cref="MediatorRegistration"/> instance so other
    ///     actions can be chained.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     if <paramref name="type"/> is a non-concrete type or is not assignable from
    ///     <see cref="IStreamHandler{TStream,TResponse}"/>.
    /// </exception>
    public MediatorRegistration WithStreamHandler(Type type) {
        Throws.When.IsNonConcreteType(type);
        Throws.When.IsNotAssignableFromGeneric(type, typeof(IStreamHandler<,>));

        _streamHandlers.Add(type);

        return this;
    }

    /// <summary>
    ///     Registers a stream pipeline behavior by generic type parameters.
    /// </summary>
    /// <typeparam name="TStreamPipelineBehavior">The stream pipeline behavior type.</typeparam>
    /// <typeparam name="TStream">The stream request type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    /// <returns>
    ///     The current <see cref="MediatorRegistration"/> instance so other
    ///     actions can be chained.
    /// </returns>
    public MediatorRegistration WithStreamPipelineBehavior<TStreamPipelineBehavior, TStream, TResponse>()
        where TStreamPipelineBehavior : IStreamPipelineBehavior<TStream, TResponse>
        where TStream : IStream<TResponse> {
        return WithStreamPipelineBehavior(typeof(TStreamPipelineBehavior));
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
    public MediatorRegistration WithStreamPipelineBehavior(Type type) {
        Throws.When.IsNonConcreteType(type);
        Throws.When.IsNotAssignableFromGeneric(type, typeof(IStreamPipelineBehavior<,>));

        if (!_streamPipelineBehaviors.Contains(type)) {
            _streamPipelineBehaviors.Add(type);
        }

        return this;
    }

    /// <summary>
    ///     Sets whether it should use the validate stream pipeline behavior.
    /// </summary>
    /// <param name="value">
    ///     The value.
    /// </param>
    /// <returns>
    ///     The current <see cref="MediatorRegistration"/> instance so other
    ///     actions can be chained.
    /// </returns>
    public MediatorRegistration WithValidateStreamPipelineBehavior(bool value) {
        UseValidateStreamPipelineBehavior = value;

        return this;
    }
}
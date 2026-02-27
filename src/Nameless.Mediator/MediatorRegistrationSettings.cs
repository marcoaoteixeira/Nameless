using Nameless.Mediator.Events;
using Nameless.Mediator.Requests;
using Nameless.Mediator.Streams;
using Nameless.Registration;

namespace Nameless.Mediator;

/// <summary>
///     Mediator options for configuring the mediator services.
/// </summary>
public class MediatorRegistrationSettings : AssemblyScanAware<MediatorRegistrationSettings> {
    private readonly HashSet<Type> _eventHandlers = [];

    private readonly HashSet<Type> _requestHandlers = [];
    private readonly List<Type> _requestPipelineBehaviors = [];
    
    private readonly HashSet<Type> _streamHandlers = [];
    private readonly List<Type> _streamPipelineBehaviors = [];

    public IReadOnlyCollection<Type> EventHandlers => UseAssemblyScan
        ? GetImplementationsFor(typeof(IEventHandler<>), includeGenericDefinition: true)
        : _eventHandlers;

    public IReadOnlyCollection<Type> RequestHandlers => UseAssemblyScan
        ? GetImplementationsFor(typeof(IRequestHandler<,>), includeGenericDefinition: true)
        : _requestHandlers;

    /// <summary>
    ///     Gets the registered request pipeline behaviors.
    /// </summary>
    public IReadOnlyCollection<Type> RequestPipelineBehaviors => _requestPipelineBehaviors;

    public IReadOnlyCollection<Type> StreamHandlers => UseAssemblyScan
        ? GetImplementationsFor(typeof(IStreamHandler<,>), includeGenericDefinition: true)
        : _streamHandlers;

    /// <summary>
    ///     Gets the registered stream pipeline behaviors.
    /// </summary>
    public IReadOnlyList<Type> StreamPipelineBehaviors => _streamPipelineBehaviors;
    
    public MediatorRegistrationSettings RegisterEventHandler<TEventHandler, TEvent>()
        where TEventHandler : IEventHandler<TEvent>
        where TEvent : IEvent {
        return RegisterEventHandler(typeof(TEventHandler));
    }

    public MediatorRegistrationSettings RegisterEventHandler(Type type) {
        Throws.When.IsNonConcreteType(type);
        Throws.When.IsNotAssignableFromGeneric(type, typeof(IEventHandler<>));

        _eventHandlers.Add(type);

        return this;
    }

    public MediatorRegistrationSettings RegisterRequestHandler<TRequestHandler, TRequest, TResponse>()
        where TRequestHandler : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse> {
        return RegisterRequestHandler(typeof(TRequestHandler));
    }

    public MediatorRegistrationSettings RegisterRequestHandler(Type type) {
        Throws.When.IsNonConcreteType(type);
        Throws.When.IsNotAssignableFromGeneric(type, typeof(IRequestHandler<,>));

        _requestHandlers.Add(type);

        return this;
    }

    public MediatorRegistrationSettings RegisterRequestPipelineBehavior<TRequestPipelineBehavior, TRequest, TResponse>()
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
    ///     The current <see cref="MediatorRegistrationSettings"/> instance so other
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
    public MediatorRegistrationSettings RegisterRequestPipelineBehavior(Type type) {
        var service = typeof(IRequestPipelineBehavior<,>);

        Throws.When.IsNonConcreteType(type);
        Throws.When.IsNotAssignableFromGeneric(type, service);

        if (!_requestPipelineBehaviors.Contains(type)) {
            _requestPipelineBehaviors.Add(type);
        }

        return this;
    }

    public MediatorRegistrationSettings RegisterStreamHandler<TStreamHandler, TStream, TResponse>()
        where TStreamHandler : IStreamHandler<TStream, TResponse>
        where TStream : IStream<TResponse> {
        return RegisterStreamHandler(typeof(TStreamHandler));
    }

    public MediatorRegistrationSettings RegisterStreamHandler(Type type) {
        Throws.When.IsNonConcreteType(type);
        Throws.When.IsNotAssignableFromGeneric(type, typeof(IStreamHandler<,>));

        _streamHandlers.Add(type);

        return this;
    }

    public MediatorRegistrationSettings RegisterStreamPipelineBehavior<TStreamPipelineBehavior, TStream, TResponse>()
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
    ///     The current <see cref="MediatorRegistrationSettings"/> instance so other
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
    public MediatorRegistrationSettings RegisterStreamPipelineBehavior(Type type) {
        var service = typeof(IStreamPipelineBehavior<,>);

        Throws.When.IsNonConcreteType(type);
        Throws.When.IsNotAssignableFromGeneric(type, service);

        if (!_streamPipelineBehaviors.Contains(type)) {
            _streamPipelineBehaviors.Add(type);
        }

        return this;
    }
}
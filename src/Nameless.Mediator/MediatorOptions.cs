using System.Reflection;
using Nameless.Mediator.Requests;
using Nameless.Mediator.Streams;

namespace Nameless.Mediator;

/// <summary>
///     Mediator options for configuring the mediator services.
/// </summary>
public sealed class MediatorOptions {
    private readonly List<Type> _requestPipelineBehaviorCollection = [];
    private readonly List<Type> _streamPipelineBehaviorCollection = [];

    /// <summary>
    ///     Gets or sets the assemblies to scan for handlers.
    /// </summary>
    public Assembly[] Assemblies { get; set; } = [];

    /// <summary>
    ///     Whether to use event handlers in the mediator.
    ///     Default is <see langword="true"/>.
    /// </summary>
    public bool UseEventHandlers { get; set; } = true;

    /// <summary>
    ///     Whether to use request handlers in the mediator.
    ///     Default is <see langword="true"/>.
    /// </summary>
    public bool UseRequestHandlers { get; set; } = true;

    /// <summary>
    ///     Whether to use stream handlers in the mediator.
    ///     Default is <see langword="true"/>.
    /// </summary>
    public bool UseStreamHandlers { get; set; } = true;

    /// <summary>
    ///     Gets the registered request pipeline behaviors.
    /// </summary>
    public Type[] RequestPipelineBehaviors => [.. _requestPipelineBehaviorCollection];

    /// <summary>
    ///     Gets the registered stream pipeline behaviors.
    /// </summary>
    public Type[] StreamPipelineBehaviors => [.. _streamPipelineBehaviorCollection];

    /// <summary>
    ///     Registers a request pipeline behavior type.
    /// </summary>
    /// <param name="pipelineBehavior">
    ///     The pipeline behavior type.
    /// </param>
    /// <returns>
    ///     The current <see cref="MediatorOptions"/> instance so other
    ///     actions can be chained.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     if <paramref name="pipelineBehavior"/> is not assignable from
    ///     <see cref="IRequestPipelineBehavior{TRequest,TResponse}"/>.
    /// </exception>
    /// <remarks>
    ///     We do not register request pipeline behavior automatically in the
    ///     service collection, that's because the pipeline behaviors need to
    ///     be registered in order so the execution of the pipeline is correct.
    /// </remarks>
    public MediatorOptions RegisterRequestPipelineBehavior(Type pipelineBehavior) {
        var service = typeof(IRequestPipelineBehavior<,>);

        if (!service.IsAssignableFromGenericType(pipelineBehavior)) {
            throw new InvalidOperationException($"Type '{pipelineBehavior.GetPrettyName()}' is not assignable from '{service.GetPrettyName()}'.");
        }

        if (!_requestPipelineBehaviorCollection.Contains(pipelineBehavior)) {
            _requestPipelineBehaviorCollection.Add(pipelineBehavior);
        }

        return this;
    }

    /// <summary>
    ///     Registers a stream pipeline behavior type.
    /// </summary>
    /// <param name="pipelineBehavior">
    ///     The pipeline behavior type.
    /// </param>
    /// <returns>
    ///     The current <see cref="MediatorOptions"/> instance so other
    ///     actions can be chained.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     if <paramref name="pipelineBehavior"/> is not assignable from
    ///     <see cref="IStreamPipelineBehavior{TRequest,TResponse}"/>.
    /// </exception>
    /// <remarks>
    ///     We do not register request pipeline behavior automatically in
    ///     the service collection, that's because the pipeline behaviors
    ///     need to be registered in order so the execution of the pipeline
    ///     is correct.
    /// </remarks>
    public MediatorOptions RegisterStreamPipelineBehavior(Type pipelineBehavior) {
        var service = typeof(IStreamPipelineBehavior<,>);

        if (!service.IsAssignableFromGenericType(pipelineBehavior)) {
            throw new InvalidOperationException($"Type '{pipelineBehavior.GetPrettyName()}' is not assignable from '{service.GetPrettyName()}'.");
        }

        if (!_streamPipelineBehaviorCollection.Contains(pipelineBehavior)) {
            _streamPipelineBehaviorCollection.Add(pipelineBehavior);
        }

        return this;
    }
}
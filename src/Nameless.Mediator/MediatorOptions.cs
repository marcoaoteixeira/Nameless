using System.Reflection;
using Nameless.Mediator.Requests;
using Nameless.Mediator.Streams;

namespace Nameless.Mediator;

/// <summary>
///     Mediator options for configuring the mediator services.
/// </summary>
public sealed class MediatorOptions {
    private readonly List<Type> _requestPipelineBehaviors = [];
    private readonly List<Type> _streamPipelineBehaviors = [];

    /// <summary>
    ///     Gets the registered request pipeline behaviors.
    /// </summary>
    internal IReadOnlyList<Type> RequestPipelineBehaviors => _requestPipelineBehaviors;

    /// <summary>
    ///     Gets the registered stream pipeline behaviors.
    /// </summary>
    internal IReadOnlyList<Type> StreamPipelineBehaviors => _streamPipelineBehaviors;

    /// <summary>
    ///     Gets or sets the assemblies to scan for handlers.
    /// </summary>
    public Assembly[] Assemblies { get; set; } = [];

    /// <summary>
    ///     Registers a request pipeline behavior type.
    /// </summary>
    /// <param name="behavior">
    ///     The pipeline behavior type.
    /// </param>
    /// <returns>
    ///     The current <see cref="MediatorOptions"/> instance so other
    ///     actions can be chained.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     if <paramref name="behavior"/> is not assignable from
    ///     <see cref="IRequestPipelineBehavior{TRequest,TResponse}"/>.
    /// </exception>
    /// <remarks>
    ///     We do not register request pipeline behavior automatically in the
    ///     service collection, that's because the pipeline behaviors need to
    ///     be registered in order so the execution of the pipeline is correct.
    /// </remarks>
    public MediatorOptions RegisterRequestPipelineBehavior(Type behavior) {
        var service = typeof(IRequestPipelineBehavior<,>);

        if (!service.IsAssignableFromGenericType(behavior)) {
            throw new InvalidOperationException($"Type '{behavior.GetPrettyName()}' is not assignable from '{service.GetPrettyName()}'.");
        }

        if (!_requestPipelineBehaviors.Contains(behavior)) {
            _requestPipelineBehaviors.Add(behavior);
        }

        return this;
    }

    /// <summary>
    ///     Registers a stream pipeline behavior type.
    /// </summary>
    /// <param name="behavior">
    ///     The pipeline behavior type.
    /// </param>
    /// <returns>
    ///     The current <see cref="MediatorOptions"/> instance so other
    ///     actions can be chained.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     if <paramref name="behavior"/> is not assignable from
    ///     <see cref="IStreamPipelineBehavior{TRequest,TResponse}"/>.
    /// </exception>
    /// <remarks>
    ///     We do not register request pipeline behavior automatically in
    ///     the service collection, that's because the pipeline behaviors
    ///     need to be registered in order so the execution of the pipeline
    ///     is correct.
    /// </remarks>
    public MediatorOptions RegisterStreamPipelineBehavior(Type behavior) {
        var service = typeof(IStreamPipelineBehavior<,>);

        if (!service.IsAssignableFromGenericType(behavior)) {
            throw new InvalidOperationException(
                $"Type '{behavior.GetPrettyName()}' is not assignable from '{service.GetPrettyName()}'.");
        }

        if (!_streamPipelineBehaviors.Contains(behavior)) {
            _streamPipelineBehaviors.Add(behavior);
        }

        return this;
    }
}
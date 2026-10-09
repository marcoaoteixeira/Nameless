using Microsoft.Extensions.DependencyInjection;

namespace Nameless.Mediator.Events;

/// <summary>
///     The default implementation of <see cref="EventHandlerWrapper" />.
/// </summary>
/// <typeparam name="TEvent">
///     Type of the event.
/// </typeparam>
internal sealed class EventHandlerWrapperImpl<TEvent> : EventHandlerWrapper
    where TEvent : IEvent {
    /// <inheritdoc />
    /// <remarks>
    ///     All handlers run concurrently. A handler failure, even one thrown
    ///     before the handler returns its task, does not stop the others.
    ///     When a single handler fails its exception is rethrown as-is;
    ///     when several fail, an <see cref="AggregateException"/> with all
    ///     of them is thrown.
    /// </remarks>
    public override async Task HandleAsync(IEvent evt, IServiceProvider provider, CancellationToken cancellationToken) {
        var tasks = provider.GetServices<IEventHandler<TEvent>>()
                            .Select(handler => InvokeAsync(handler, (TEvent)evt, cancellationToken))
                            .ToArray();

        var all = Task.WhenAll(tasks);

        try { await all.SkipContextSync(); }
        catch when (all.Exception is { InnerExceptions.Count: > 1 }) {
            throw all.Exception;
        }
    }

    // Wrapping the call in an async method turns an exception thrown before
    // the handler returns its task into a faulted task.
    private static async Task InvokeAsync(IEventHandler<TEvent> handler, TEvent evt, CancellationToken cancellationToken) {
        await handler.HandleAsync(evt, cancellationToken).SkipContextSync();
    }
}

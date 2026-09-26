namespace Nameless.EventSourcing.Projections;

/// <summary>
///     Rebuilds one or more <see cref="IProjection"/>s from scratch by
///     replaying every event in the store, in global order. This is
///     currently the only mechanism that drives a projection — see
///     <see cref="IProjection"/>.
/// </summary>
public sealed class ProjectionRebuilder {
    private readonly IEventStore _eventStore;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ProjectionRebuilder"/>
    ///     class.
    /// </summary>
    /// <param name="eventStore">
    ///     The store to replay events from.
    /// </param>
    public ProjectionRebuilder(IEventStore eventStore) {
        _eventStore = Throws.When.Null(eventStore);
    }

    /// <summary>
    ///     Resets <paramref name="projections"/> and replays every event
    ///     in the store through them, in global order.
    /// </summary>
    /// <param name="projections">
    ///     The projections to rebuild.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task"/> representing the rebuild operation.
    /// </returns>
    public async Task RebuildAsync(IReadOnlyCollection<IProjection> projections, CancellationToken cancellationToken) {
        Throws.When.Null(projections);

        foreach (var projection in projections) {
            await projection.ResetAsync(cancellationToken);
        }

        // Drain the stream fully before applying anything: a projection
        // may write through the same DbContext/connection as the event
        // store, and most providers (Npgsql included) reject a second
        // command on a connection while a prior query's reader is still
        // open.
        var envelopes = new List<EventEnvelope>();
        await foreach (var envelope in _eventStore.GetAsync(fromGlobalSequence: 0, cancellationToken)) {
            envelopes.Add(envelope);
        }

        foreach (var envelope in envelopes) {
            foreach (var projection in projections) {
                await projection.ApplyAsync(envelope, cancellationToken);
            }
        }
    }
}
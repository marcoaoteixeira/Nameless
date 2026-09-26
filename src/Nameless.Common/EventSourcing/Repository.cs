using Nameless.EventSourcing.UpCasting;
using Nameless.Mediator;
using Nameless.Resilience;

namespace Nameless.EventSourcing;

/// <summary>
///     Loads and persists event-sourced aggregates of type
///     <typeparamref name="TAggregate"/>, retrying command execution on
///     optimistic concurrency conflicts and publishing newly-appended
///     events once they are durably stored.
/// </summary>
/// <typeparam name="TAggregate">
///     Type of the aggregate this repository manages.
/// </typeparam>
/// <remarks>
///     The aggregate's stream discriminator (<see cref="EventEnvelope.AggregateType"/>)
///     defaults to <c>typeof(TAggregate).Name</c>.
/// </remarks>
public sealed class Repository<TAggregate> where TAggregate : AggregateRoot<Guid>, new() {
    private static readonly string AggregateType = typeof(TAggregate).Name;

    private readonly IEventSerializer _eventSerializer;
    private readonly IEventStore _eventStore;
    private readonly IMediator _mediator;
    private readonly IRetryPipelineFactory _retryPipelineFactory;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Repository{TAggregate}"/>
    ///     class.
    /// </summary>
    /// <param name="eventSerializer">
    ///     Serializes/deserializes the aggregate's events.
    /// </param>
    /// <param name="eventStore">
    ///     The store events are appended to and read from.
    /// </param>
    /// <param name="mediator">
    ///     Publishes newly-appended events, e.g. for projections/reactions.
    /// </param>
    /// <param name="retryPipelineFactory">
    ///     Builds the retry pipeline used to reload and reapply a
    ///     command when a <see cref="ConcurrencyConflictException"/>
    ///     occurs.
    /// </param>
    public Repository(IEventSerializer eventSerializer, IEventStore eventStore, IMediator mediator, IRetryPipelineFactory retryPipelineFactory) {
        _eventSerializer = Throws.When.Null(eventSerializer);
        _eventStore = Throws.When.Null(eventStore);
        _mediator = Throws.When.Null(mediator);
        _retryPipelineFactory = Throws.When.Null(retryPipelineFactory);
    }

    /// <summary>
    ///     Loads the aggregate identified by <paramref name="aggregateID"/> by
    ///     replaying its full event history.
    /// </summary>
    /// <param name="aggregateID">
    ///     The aggregate's identifier.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     The loaded aggregate, or <see langword="null"/> when no
    ///     events have been recorded for <paramref name="aggregateID"/>.
    /// </returns>
    public async Task<TAggregate?> LoadAsync(Guid aggregateID, CancellationToken cancellationToken) {
        var streamID = GetStreamID(aggregateID);

        var envelopes = await _eventStore.ReadStreamAsync(streamID, cancellationToken).SkipContextSync();
        if (envelopes.Count == 0) { return null; }

        var events = envelopes.Select(
            envelope => _eventSerializer.Deserialize(
                new EventMetadata(envelope.EventType, envelope.EventSchemaVersion, envelope.Payload)
            )
        );

        var aggregate = new TAggregate();

        aggregate.LoadFromHistory(events);

        return aggregate;
    }

    /// <summary>
    ///     Persists <paramref name="aggregate"/>'s uncommitted events —
    ///     the newly-raised events of a brand-new aggregate, or of one
    ///     previously loaded via <see cref="LoadAsync"/>. Unlike
    ///     <see cref="ExecuteAsync{TCommand}"/>, this does not retry on
    ///     conflict: a <see cref="ConcurrencyConflictException"/>
    ///     propagates directly to the caller.
    /// </summary>
    /// <param name="aggregate">
    ///     The aggregate to persist.
    /// </param>
    /// <param name="actor">
    ///     The actor responsible for the uncommitted events.
    /// </param>
    /// <param name="correlationID">
    ///     The identifier correlating the events with this operation.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    public async Task SaveAsync(TAggregate aggregate, ActorContext actor, Guid correlationID, CancellationToken cancellationToken) {
        Throws.When.Null(aggregate);
        Throws.When.Null(actor);

        var uncommittedEvents = aggregate.UncommittedEvents;
        if (uncommittedEvents.Count == 0) { return; }

        var expectedVersion = aggregate.Version - uncommittedEvents.Count;
        var streamID = GetStreamID(aggregate.ID);

        await _eventStore.AppendAsync(
            streamID,
            AggregateType,
            aggregate.ID,
            actor.TenantID,
            expectedVersion,
            uncommittedEvents,
            actor.UserID,
            correlationID,
            cancellationToken
        ).SkipContextSync();

        aggregate.ClearUncommittedEvents();

        foreach (var @event in uncommittedEvents) {
            await _mediator.PublishAsync(@event, cancellationToken).SkipContextSync();
        }
    }

    /// <summary>
    ///     Loads the aggregate identified by <paramref name="aggregateID"/>,
    ///     checks <paramref name="policy"/>, applies <paramref name="command"/>
    ///     via <paramref name="apply"/>, and persists the resulting
    ///     events. Retries the whole load-authorize-apply-append sequence
    ///     against fresh state when a <see cref="ConcurrencyConflictException"/>
    ///     occurs.
    /// </summary>
    /// <typeparam name="TCommand">
    ///     Type of the command being executed.
    /// </typeparam>
    /// <param name="aggregateID">
    ///     The aggregate's identifier.
    /// </param>
    /// <param name="command">
    ///     The command to execute.
    /// </param>
    /// <param name="apply">
    ///     Applies <paramref name="command"/> to the loaded aggregate.
    /// </param>
    /// <param name="actor">
    ///     The actor executing <paramref name="command"/>.
    /// </param>
    /// <param name="correlationID">
    ///     The identifier correlating the resulting events with this
    ///     operation.
    /// </param>
    /// <param name="policy">
    ///     The policy authorizing <paramref name="actor"/> to execute
    ///     <paramref name="command"/>, or <see langword="null"/> to skip
    ///     authorization.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when no aggregate is found for <paramref name="aggregateID"/>.
    /// </exception>
    /// <exception cref="UnauthorizedActionException">
    ///     Thrown when <paramref name="policy"/> denies <paramref name="actor"/>.
    /// </exception>
    public async Task ExecuteAsync<TCommand>(
        Guid aggregateID,
        TCommand command,
        Action<TAggregate, TCommand> apply,
        ActorContext actor,
        Guid correlationID,
        IAuthorizationPolicy<TAggregate>? policy,
        CancellationToken cancellationToken) {
        Throws.When.Null(command);
        Throws.When.Null(apply);
        Throws.When.Null(actor);

        var pipeline = _retryPipelineFactory.Create(new RetryPolicyConfiguration {
            Tag = $"EventSourcing.{AggregateType}",
            RetryCount = 3,
            InitialDelay = TimeSpan.FromMilliseconds(milliseconds: 50),
            BackoffType = BackoffType.Exponential,
            MaxDelay = TimeSpan.FromSeconds(seconds: 1),
            UseJitter = true,
            RetryOnException = static ex => ex is ConcurrencyConflictException,
            OnRetry = (_, _, _, _) => { }
        });

        var appendedEvents = await pipeline.ExecuteAsync(async token => {
            var aggregate = await LoadAsync(aggregateID, token).SkipContextSync() ??
                            throw new InvalidOperationException(
                                $"Aggregate '{AggregateType}' with id '{aggregateID}' was not found."
                            );

            if (policy is not null && !policy.CanApply(aggregate, command, actor)) {
                throw new UnauthorizedActionException(actor, command);
            }

            apply(aggregate, command);

            var uncommittedEvents = aggregate.UncommittedEvents;
            if (uncommittedEvents.Count == 0) { return uncommittedEvents; }

            var expectedVersion = aggregate.Version - uncommittedEvents.Count;
            var streamID = GetStreamID(aggregateID);

            await _eventStore.AppendAsync(
                streamID,
                AggregateType,
                aggregateID,
                actor.TenantID,
                expectedVersion,
                uncommittedEvents,
                actor.UserID,
                correlationID,
                token
            ).SkipContextSync();

            aggregate.ClearUncommittedEvents();

            return uncommittedEvents;
        }, cancellationToken);

        foreach (var @event in appendedEvents) {
            await _mediator.PublishAsync(@event, cancellationToken).SkipContextSync();
        }
    }

    private static string GetStreamID(Guid id) {
        return $"{AggregateType}-{id}";
    }
}
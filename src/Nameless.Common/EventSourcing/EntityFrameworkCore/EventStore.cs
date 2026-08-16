using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Nameless.EventSourcing.UpCasting;
using Nameless.Mediator.Events;

namespace Nameless.EventSourcing.EntityFrameworkCore;

/// <summary>
///     EF Core-backed <see cref="IEventStore"/>, persisting
///     <see cref="EventEnvelope"/> rows through the consuming
///     application's own <typeparamref name="TDbContext"/>. Recognizes
///     unique-constraint violations from the Postgres (<c>Npgsql</c>)
///     provider directly, and from SQLite (<c>Microsoft.Data.Sqlite</c>)
///     via reflection — so this library doesn't need a compile-time
///     dependency on the SQLite provider just to support it as an
///     alternative for consuming applications.
/// </summary>
/// <typeparam name="TDbContext">
///     Type of the consuming application's <see cref="DbContext"/>,
///     which must expose <c>DbSet&lt;EventEnvelope&gt;</c> and apply
///     <see cref="EventEnvelopeEntityTypeConfiguration"/>.
/// </typeparam>
public sealed class EventStore<TDbContext> : IEventStore
    where TDbContext : DbContext {
    private const string POSTGRES_EXCEPTION_TYPE_FULL_NAME = "Npgsql.PostgresException";
    private const string POSTGRES_EXCEPTION_ERROR_CODE_PROPERTY = "SqlState";
    private const string POSTGRES_UNIQUE_VIOLATION_STATE = "23505";

    private const string SQLITE_EXCEPTION_TYPE_FULL_NAME = "Microsoft.Data.Sqlite.SqliteException";
    private const string SQLITE_EXCEPTION_ERROR_CODE_PROPERTY = "SqliteExtendedErrorCode";
    private const int SQLITE_UNIQUE_VIOLATION_STATE = 2067;

    private readonly TDbContext _dbContext;
    private readonly IEventSerializer _eventSerializer;

    /// <summary>
    ///     Initializes a new instance of the <see cref="EventStore{TDbContext}"/>
    ///     class.
    /// </summary>
    /// <param name="dbContext">
    ///     The consuming application's <see cref="DbContext"/>.
    /// </param>
    /// <param name="eventSerializer">
    ///     Serializes events into their persisted representation.
    /// </param>
    public EventStore(TDbContext dbContext, IEventSerializer eventSerializer) {
        _dbContext = Throws.When.Null(dbContext);
        _eventSerializer = Throws.When.Null(eventSerializer);
    }

    /// <inheritdoc/>
    public async Task AppendAsync(
        string streamID,
        string aggregateType,
        Guid aggregateID,
        Guid? tenantID,
        int expectedVersion,
        IReadOnlyList<IEvent> events,
        Guid causedBy,
        Guid correlationID,
        CancellationToken cancellationToken) {
        Throws.When.NullOrWhiteSpace(streamID);
        Throws.When.NullOrWhiteSpace(aggregateType);
        Throws.When.Null(events);

        if (events.Count == 0) { return; }

        var occurredAt = DateTimeOffset.UtcNow;
        var version = expectedVersion;
        var envelopes = new List<EventEnvelope>(events.Count);

        foreach (var @event in events) {
            version++;

            var (eventType, schemaVersion, payload) = _eventSerializer.Serialize(@event);

            envelopes.Add(new EventEnvelope {
                EventID = Guid.CreateVersion7(),
                StreamID = streamID,
                AggregateType = aggregateType,
                AggregateID = aggregateID,
                Version = version,
                GlobalSequence = 0, // database-generated
                EventType = eventType,
                EventSchemaVersion = schemaVersion,
                Payload = payload,
                TenantID = tenantID,
                CausedBy = causedBy,
                CorrelationID = correlationID,
                OccurredAt = occurredAt
            });
        }

        _dbContext.Set<EventEnvelope>().AddRange(envelopes);

        try { await _dbContext.SaveChangesAsync(cancellationToken).SkipContextSync(); }
        catch (DbUpdateException ex) when (IsPostgresUniqueViolation(ex) || IsSqliteUniqueViolation(ex)) {
            throw new ConcurrencyConflictException(streamID, expectedVersion);
        }
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<EventEnvelope>> ReadStreamAsync(string streamID,
        CancellationToken cancellationToken) {
        Throws.When.NullOrWhiteSpace(streamID);

        return await _dbContext
            .Set<EventEnvelope>()
            .AsNoTracking()
            .Where(envelope => envelope.StreamID == streamID)
            .OrderBy(envelope => envelope.Version)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<EventEnvelope> GetAsync(long fromGlobalSequence, [EnumeratorCancellation] CancellationToken cancellationToken) {
        var query = _dbContext
            .Set<EventEnvelope>()
            .AsNoTracking()
            .Where(envelope => envelope.GlobalSequence > fromGlobalSequence)
            .OrderBy(envelope => envelope.GlobalSequence)
            .AsAsyncEnumerable();

        await foreach (var envelope in query.WithCancellation(cancellationToken)) {
            yield return envelope;
        }
    }

    // Checked by type name via reflection, rather than a direct reference
    // to Npgsql, so this library has no compile-time dependency on the
    // Postgres provider.
    private static bool IsPostgresUniqueViolation(Exception? innerException) {
        if (innerException is null || innerException.GetType().FullName != POSTGRES_EXCEPTION_TYPE_FULL_NAME) {
            return false;
        }

        var property = innerException
            .GetType()
            .GetProperty(POSTGRES_EXCEPTION_ERROR_CODE_PROPERTY)?
            .GetValue(innerException);

        return property is POSTGRES_UNIQUE_VIOLATION_STATE;
    }

    // Checked by type name via reflection, rather than a direct reference
    // to Microsoft.Data.Sqlite, so this library has no compile-time
    // dependency on the SQLite provider.
    private static bool IsSqliteUniqueViolation(Exception? innerException) {
        if (innerException is null || innerException.GetType().FullName != SQLITE_EXCEPTION_TYPE_FULL_NAME) {
            return false;
        }

        var property = innerException
            .GetType()
            .GetProperty(SQLITE_EXCEPTION_ERROR_CODE_PROPERTY)?
            .GetValue(innerException);

        return property is SQLITE_UNIQUE_VIOLATION_STATE;
    }
}
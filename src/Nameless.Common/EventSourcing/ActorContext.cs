namespace Nameless.EventSourcing;

/// <summary>
///     Identifies the actor executing a command against an aggregate,
///     supplied explicitly by the caller for authorization checks and
///     event metadata (<see cref="EventEnvelope.CausedBy"/>).
/// </summary>
public sealed record ActorContext {
    /// <summary>
    ///     Gets the identifier of the user or process executing the
    ///     command.
    /// </summary>
    public Guid UserID { get; }

    /// <summary>
    ///     Gets the identifier of the tenant the command is scoped to,
    ///     if any.
    /// </summary>
    public Guid? TenantID { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ActorContext"/>
    ///     class.
    /// </summary>
    /// <param name="userID">
    ///     The identifier of the user or process executing the command.
    /// </param>
    /// <param name="tenantID">
    ///     The identifier of the tenant the command is scoped to, if
    ///     any.
    /// </param>
    public ActorContext(Guid userID, Guid? tenantID = null) {
        UserID = userID;
        TenantID = tenantID;
    }
}
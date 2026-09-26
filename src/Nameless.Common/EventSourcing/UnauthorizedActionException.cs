namespace Nameless.EventSourcing;

/// <summary>
///     Exception thrown when an <see cref="IAuthorizationPolicy{TAggregate}"/>
///     denies an actor's command against an aggregate.
/// </summary>
public sealed class UnauthorizedActionException : Exception {
    /// <summary>
    ///     Initializes a new instance of the
    ///     <see cref="UnauthorizedActionException"/> class.
    /// </summary>
    /// <param name="actor">
    ///     The actor that was denied.
    /// </param>
    /// <param name="command">
    ///     The command that was denied.
    /// </param>
    public UnauthorizedActionException(ActorContext actor, object command)
        : base($"Actor '{actor.UserID}' is not authorized to execute '{command.GetType().Name}'.") {
        Actor = actor;
        Command = command;
    }

    /// <summary>
    ///     Gets the actor that was denied.
    /// </summary>
    public ActorContext Actor { get; }

    /// <summary>
    ///     Gets the command that was denied.
    /// </summary>
    public object Command { get; }
}
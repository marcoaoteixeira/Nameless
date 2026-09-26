namespace Nameless.EventSourcing;

/// <summary>
///     Decides whether an actor is allowed to execute a command against
///     an aggregate. Enforces authorization as a seam separate from the
///     aggregate's own state invariants: the aggregate never needs to
///     know about roles or permissions.
/// </summary>
/// <typeparam name="TAggregate">
///     Type of the aggregate the policy applies to.
/// </typeparam>
public interface IAuthorizationPolicy<in TAggregate> {
    /// <summary>
    ///     Determines whether <paramref name="actor"/> is allowed to
    ///     execute <paramref name="command"/> against <paramref name="state"/>.
    /// </summary>
    /// <param name="state">
    ///     The aggregate's current state.
    /// </param>
    /// <param name="command">
    ///     The command about to be executed.
    /// </param>
    /// <param name="actor">
    ///     The actor executing the command.
    /// </param>
    /// <returns>
    ///     <see langword="true"/> if <paramref name="actor"/> is allowed
    ///     to execute <paramref name="command"/>; otherwise, <see langword="false"/>.
    /// </returns>
    bool CanApply(TAggregate state, object command, ActorContext actor);
}
namespace Nameless.EventSourcing.Projections;

/// <summary>
///     Builds a read model by folding events. Nothing currently drives
///     <see cref="ApplyAsync"/> as events are appended — the only caller
///     today is <see cref="ProjectionRebuilder"/>, which resets and
///     replays the full event history. Wiring live updates (e.g. via
///     <c>IMediator</c>) is not yet implemented.
/// </summary>
public interface IProjection {
    /// <summary>
    ///     Clears the projection's read model back to its initial,
    ///     empty state. Called once before replaying events during a
    ///     rebuild.
    /// </summary>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task"/> representing the reset operation.
    /// </returns>
    Task ResetAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Folds <paramref name="envelope"/> into the projection's read
    ///     model.
    /// </summary>
    /// <param name="envelope">
    ///     The event to apply.
    /// </param>
    /// <param name="cancellationToken">
    ///     The cancellation token.
    /// </param>
    /// <returns>
    ///     A <see cref="Task"/> representing the apply operation.
    /// </returns>
    Task ApplyAsync(EventEnvelope envelope, CancellationToken cancellationToken);
}
namespace Nameless.Workers.Notification;

/// <summary>
///     Categorises a <see cref="WorkerProgress" /> notification emitted
///     during a worker's execution cycle.
/// </summary>
public enum WorkerProgressType {
    /// <summary>
    ///     A general informational message produced from within
    ///     <see cref="Worker.DoWorkAsync" />.
    /// </summary>
    Information,

    /// <summary>
    ///     Emitted at the beginning of a timer tick, before
    ///     <see cref="Worker.DoWorkAsync" /> is called.
    /// </summary>
    TickStarted,

    /// <summary>
    ///     Emitted when <see cref="Worker.DoWorkAsync" /> returns successfully.
    /// </summary>
    TickCompleted,

    /// <summary>
    ///     Emitted when <see cref="Worker.DoWorkAsync" /> throws an unhandled
    ///     exception.
    /// </summary>
    TickFailed,

    /// <summary>
    ///     Emitted when the worker's cancellation token is signalled and the
    ///     execution loop exits cleanly.
    /// </summary>
    Cancelled,
}

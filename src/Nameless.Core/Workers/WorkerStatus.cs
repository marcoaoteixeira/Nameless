namespace Nameless.Workers;

/// <summary>
///     Represents the execution status of a <see cref="Worker" />.
/// </summary>
public enum WorkerStatus {
    /// <summary>
    ///     The worker is initialized and waiting for the next timer tick.
    ///     This is also the state the worker returns to after each tick
    ///     completes normally.
    /// </summary>
    Idle,

    /// <summary>
    ///     The worker is actively executing <see cref="Worker.DoWorkAsync" />.
    /// </summary>
    Running,

    /// <summary>
    ///     An unhandled exception escaped <see cref="Worker.DoWorkAsync" />
    ///     and was caught by the hosting loop. The worker has stopped.
    /// </summary>
    Faulted,

    /// <summary>
    ///     The cancellation token was signalled and the timer loop exited
    ///     cleanly. The worker has stopped.
    /// </summary>
    Stopped,
}

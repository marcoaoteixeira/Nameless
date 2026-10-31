using Nameless.Windows.Notification;

namespace Nameless.Windows.TaskRunner;

/// <summary>
///     Task Runner Build Contract
/// </summary>
/// <remarks>
///     May be called from any thread. A builder instance is not meant for
///     concurrent use from several threads at the same time. Calls that
///     create the window marshal to the application dispatcher. Do not
///     block the UI thread waiting on a builder call made from another
///     thread (deadlock).
/// </remarks>
public interface ITaskRunnerBuilder {
    /// <summary>
    ///     Sets the task runner window name.
    /// </summary>
    /// <param name="name">
    ///     The name.
    /// </param>
    /// <returns>
    ///     The current <see cref="ITaskRunnerBuilder"/> instance
    ///     so other actions can be chained.
    /// </returns>
    ITaskRunnerBuilder SetName(string name);

    /// <summary>
    ///     Sets the delegate that will be executed by the task runner window.
    /// </summary>
    /// <param name="delegate">
    ///     The delegate.
    /// </param>
    /// <returns>
    ///     The current <see cref="ITaskRunnerBuilder"/> instance
    ///     so other actions can be chained.
    /// </returns>
    ITaskRunnerBuilder SetDelegate(TaskRunnerDelegate @delegate);

    /// <summary>
    ///     Subscribes the task runner window for notification.
    /// </summary>
    /// <typeparam name="TNotificationEvent">
    ///     Type of the notification event.
    /// </typeparam>
    /// <returns>
    ///     The current <see cref="ITaskRunnerBuilder"/> instance
    ///     so other actions can be chained.
    /// </returns>
    ITaskRunnerBuilder SubscribeFor<TNotificationEvent>() where TNotificationEvent : NotificationEvent;

    /// <summary>
    ///     Shows the task runner window and waits for the run to finish.
    /// </summary>
    /// <returns>
    ///     A <see cref="Task{TResult}"/> that completes with
    ///     <see cref="TaskRunnerOutcome.Completed"/> or
    ///     <see cref="TaskRunnerOutcome.Cancelled"/>, or that faults with
    ///     the delegate's own exception (user cancellation of the run's own
    ///     token is reported as <see cref="TaskRunnerOutcome.Cancelled"/>,
    ///     not as a fault).
    /// </returns>
    /// <remarks>
    ///     A builder runs once: the window is single-use and a second call
    ///     to <see cref="RunAsync"/> on the same builder throws
    ///     <see cref="InvalidOperationException"/>. Create a new builder
    ///     via <see cref="ITaskRunner.CreateBuilder"/> for another run.
    /// </remarks>
    Task<TaskRunnerOutcome> RunAsync();
}
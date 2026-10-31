using Nameless.Windows.Notification;

namespace Nameless.Windows.TaskRunner;

/// <summary>
///     Task Runner Build Contract
/// </summary>
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
    ///     Runs the task runner window.
    /// </summary>
    /// <returns>
    ///     A <see cref="Task"/> representing the asynchronous method execution.
    /// </returns>
    Task RunAsync();
}
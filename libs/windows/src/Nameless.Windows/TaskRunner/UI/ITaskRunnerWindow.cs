using Nameless.Windows.Notification;
using Nameless.Windows.UI;
using System.Windows;

namespace Nameless.Windows.TaskRunner.UI;

/// <summary>
///     Task Runner Window
/// </summary>
public interface ITaskRunnerWindow : IWindow {
    /// <summary>
    ///     Gets the view model.
    /// </summary>
    TaskRunnerWindowViewModel ViewModel { get; }

    /// <summary>
    ///     Sets the name of the window.
    /// </summary>
    /// <param name="name">
    ///     The name.
    /// </param>
    /// <returns>
    ///     The current <see cref="ITaskRunnerWindow"/> instance so
    ///     other actions can be chained.
    /// </returns>
    ITaskRunnerWindow SetName(string name);

    /// <summary>
    ///     Sets the delegate that will be executed by the task runner window.
    /// </summary>
    /// <param name="delegate">
    ///     The delegate.
    /// </param>
    ITaskRunnerWindow SetDelegate(TaskRunnerDelegate @delegate);

    /// <summary>
    ///     Subscribes for events.
    /// </summary>
    /// <typeparam name="TNotificationEvent">
    ///     Type of the event.
    /// </typeparam>
    ITaskRunnerWindow SubscribeFor<TNotificationEvent>() where TNotificationEvent : NotificationEvent;

    /// <summary>
    ///     Sets the dialog owner.
    /// </summary>
    /// <param name="owner">
    ///     The dialog owner.
    /// </param>
    /// <returns>
    ///     The current <see cref="ITaskRunnerWindow"/> instance so
    ///     other actions can be chained.
    /// </returns>
    ITaskRunnerWindow SetOwner(Window? owner);
}
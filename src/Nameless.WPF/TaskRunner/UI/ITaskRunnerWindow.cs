using System.Windows;
using Nameless.WPF.Messaging;
using Nameless.WPF.Windows;

namespace Nameless.WPF.TaskRunner.UI;

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
    ///     Sets the 
    /// </summary>
    /// <param name="delegate">
    ///     The delegate.
    /// </param>
    ITaskRunnerWindow SetDelegate(TaskRunnerDelegate @delegate);

    /// <summary>
    ///     Subscribes for events.
    /// </summary>
    /// <typeparam name="TMessage">
    ///     Type of the event.
    /// </typeparam>
    ITaskRunnerWindow SubscribeFor<TMessage>() where TMessage : Message;

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
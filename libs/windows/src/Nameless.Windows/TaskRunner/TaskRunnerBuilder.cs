using Nameless.Contracts;
using Nameless.Windows.Notification;
using Nameless.Windows.TaskRunner.UI;
using Nameless.Windows.UI;
using System.Windows;

namespace Nameless.Windows.TaskRunner;

/// <summary>
///     Task Runner Builder
/// </summary>
public class TaskRunnerBuilder : ITaskRunnerBuilder {
    private readonly IWindowFactory _windowFactory;
    private readonly Lazy<ITaskRunnerWindow> _taskRunnerWindow;

    private ITaskRunnerWindow Window => _taskRunnerWindow.Value;

    /// <summary>
    ///     Initializes a new <see cref="TaskRunnerBuilder"/> class.
    /// </summary>
    /// <param name="windowFactory">
    ///     The window factory.
    /// </param>
    public TaskRunnerBuilder(IWindowFactory windowFactory) {
        _windowFactory = Throws.When.Null(windowFactory);
        _taskRunnerWindow = new Lazy<ITaskRunnerWindow>(CreateTaskRunnerWindow);
    }

    /// <inheritdoc/>
    public ITaskRunnerBuilder SetName(string name) {
        Window.SetName(name);

        return this;
    }

    /// <inheritdoc/>
    public ITaskRunnerBuilder SetDelegate(TaskRunnerDelegate @delegate) {
        Window.SetDelegate(@delegate);

        return this;
    }

    /// <inheritdoc/>
    public ITaskRunnerBuilder SubscribeFor<TNotificationEvent>() where TNotificationEvent : NotificationEvent {
        Window.SubscribeFor<TNotificationEvent>();

        return this;
    }

    /// <inheritdoc/>
    public Task RunAsync() {
        Window.Show(WindowStartupLocation.CenterScreen);

        return Task.CompletedTask;
    }

    private ITaskRunnerWindow CreateTaskRunnerWindow() {
        var window = _windowFactory.Create<ITaskRunnerWindow>();

        window.SetOwner(WindowsApplication.Current.MainWindow);

        return window;
    }
}